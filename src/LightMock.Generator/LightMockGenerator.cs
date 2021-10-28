/******************************************************************************
    MIT License

    Copyright (c) 2021 Anton Yashin

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*******************************************************************************
    https://github.com/anton-yashin/
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using LightMock.Generator.Locators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace LightMock.Generator
{
    [Generator]
#if ROSLYN_4
    public class LightMockGenerator : IIncrementalGenerator
#else
    public class LightMockGenerator : ISourceGenerator
#endif
    {
        private readonly TypeMatcher mockContextMatcher;
        private readonly TypeMatcher mockInterfaceMatcher;
        private readonly string multicastDelegateNameSpaceAndName;

        public LightMockGenerator()
        {
            mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            mockInterfaceMatcher = new TypeMatcher(typeof(IAdvancedMockContext<>));
            var multicastDelegateType = typeof(MulticastDelegate);
            multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;
        }

#if ROSLYN_4 == false

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation is CSharpCompilation compilation == false)
                return;
            if (context.ParseOptions is CSharpParseOptions parseOptions == false)
                return;
            if (context.SyntaxContextReceiver is LightMockSyntaxReceiver receiver == false)
                return;
            if (IsGenerationDisabledByOptions(context.AnalyzerConfigOptions) || receiver.DisableCodeGeneration)
                return;

            var cc = new CodeGenerationContext(context, compilation, parseOptions);

            cc = DoGenerateCode(
                cc,
                receiver.AbstractClasses.Select(
                    t => new AbstractClassProcessor(
                        t.mock, t.mockedType, receiver.DontOverrideTypes)),
                context.CancellationToken);
            cc = DoGenerateCode(
                cc,
                receiver.Interfaces.Select(t => new InterfaceProcessor(t)),
                context.CancellationToken);
            cc = DoGenerateCode(
                cc,
                receiver.Delegates.Select(t => new DelegateProcessor(t)),
                context.CancellationToken);
            cc = DoGenerate(
                cc,
                receiver.ArrangeInvocations.ToImmutableArray(),
                context.CancellationToken);
        }

#endif

        CodeGenerationContext DoGenerate(
            CodeGenerationContext context,
            ImmutableArray<InvocationExpressionSyntax> arrangeInvocations,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // process symbols under ArrangeSetter

            var mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            var processedFiles = new HashSet<string>();
            var mockInterfaceMatcher = new TypeMatcher(typeof(IAdvancedMockContext<>));
            foreach (var candidateInvocation in arrangeInvocations)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var st = context.Compilation.GetSemanticModel(candidateInvocation.SyntaxTree);
                var methodSymbol = st.GetSymbolInfo(candidateInvocation, cancellationToken).Symbol as IMethodSymbol;
                cancellationToken.ThrowIfCancellationRequested();

                if (methodSymbol != null
                    && (mockContextMatcher.IsMatch(methodSymbol.ContainingType)
                        || mockInterfaceMatcher.IsMatch(methodSymbol.ContainingType)))
                {
                    ExpressionRewriter processor;
                    switch (methodSymbol.Name)
                    {
                        case nameof(AbstractMockNameofProvider.ArrangeSetter):
                            processor = new ArrangeExpressionRewriter(methodSymbol, candidateInvocation, context.Compilation);
                            break;
                        case nameof(AbstractMockNameofProvider.AssertSet):
                            processor = new AssertExpressionRewriter(methodSymbol, candidateInvocation, context.Compilation);
                            break;
                        default:
                            continue;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    if (processedFiles.Contains(processor.FileName))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticsDescriptors.KPropertyExpressionMustHaveUniqueId,
                            candidateInvocation.GetLocation(), methodSymbol.Name));
                        continue;
                    }
                    if (context.EmitDiagnostics(processor.GetErrors()))
                        continue;
                    cancellationToken.ThrowIfCancellationRequested();
                    context.EmitDiagnostics(processor.GetWarnings());
                    cancellationToken.ThrowIfCancellationRequested();
                    var text = processor.DoGenerate();
                    context.AddSource(processor.FileName, text);
                }
            }
            return context;
        }

        CodeGenerationContext DoGenerateCode(
            CodeGenerationContext context,
            IEnumerable<ClassProcessor> classProcessors,
            CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var classProcessor in classProcessors)
            {
                context = DoGenerateCode(
                    context,
                    classProcessor,
                    cancellationToken);
            }
            return context;
        }


        CodeGenerationContext DoGenerateCode(
            CodeGenerationContext context,
            ClassProcessor classProcessor,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (context.EmitDiagnostics(classProcessor.GetErrors()))
                return context;
            cancellationToken.ThrowIfCancellationRequested();
            context.EmitDiagnostics(classProcessor.GetWarnings());
            var text = classProcessor.DoGenerate();
            context.AddSource(classProcessor.FileName, text);
            if (classProcessor.IsUpdateCompilationRequired)
            {
                context = context.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                    text, context.ParseOptions, cancellationToken: cancellationToken)
                    );
            }
            cancellationToken.ThrowIfCancellationRequested();
            return context;
        }

        bool IsGenerationDisabledByOptions(AnalyzerConfigOptionsProvider optionsProvider)
            => optionsProvider.GlobalOptions.TryGetValue(GlobalOptionsNames.Enable, out var value)
                && value.Equals("false", StringComparison.InvariantCultureIgnoreCase);

        bool EmitDiagnostics<TContext>(TContext context, Action<TContext, Diagnostic> reportDiagnostic, IEnumerable<Diagnostic> diagnostics)
        {
            bool haveIssues = false;
            foreach (var d in diagnostics)
            {
                haveIssues = true;
                reportDiagnostic(context, d);
            }
            return haveIssues;
        }

#if ROSLYN_4

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var interfaces1 = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is ObjectCreationExpressionSyntax { Type: GenericNameSyntax gns}
                && LightMockSyntaxReceiver.IsMock(gns),
                (ctx, ct) => ConvertToInterface(ctx));
            var interfaces2 = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is ObjectCreationExpressionSyntax { Type: QualifiedNameSyntax { Right: GenericNameSyntax gns } }
                && LightMockSyntaxReceiver.IsMock(gns),
                (ctx, ct) => ConvertToInterface(ctx));
            var disableCodegenerationAttributes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDisableCodeGenerationAttribute(@as),
                (ctx, ct) => LightMockSyntaxReceiver.IsDisableCodeGenerationAttribute(ctx.SemanticModel, (AttributeSyntax)ctx.Node));
            context.RegisterSourceOutput(interfaces1
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Select((comb, ct) => (candidate: comb.Left.Left, compilation: comb.Left.Right, options: comb.Right))
                .Combine(disableCodegenerationAttributes.Collect())
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, disableCodegenerationAttributes: comb.Right))
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, comb.Left.disableCodegenerationAttributes, parseOptions: comb.Right))
                .Where(t => t.disableCodegenerationAttributes.Where(t => t == true).Any() == false
                && t.candidate != null && IsGenerationDisabledByOptions(t.options) == false
                && t.compilation is CSharpCompilation && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp, (CSharpCompilation)sr.compilation, (CSharpParseOptions)sr.parseOptions),
                    new InterfaceProcessor(sr.candidate!),
                    sp.CancellationToken));

            //var candidateMocks = context.SyntaxProvider.CreateSyntaxProvider(
            //    (sn, ct) => sn is GenericNameSyntax gns && LightMockSyntaxReceiver.IsMock(gns),
            //    (ctx, ct) => (GenericNameSyntax)ctx.Node);
            //var disableCodegenerationAttributes = context.SyntaxProvider.CreateSyntaxProvider(
            //    (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDisableCodeGenerationAttribute(@as),
            //    (ctx, ct) => (AttributeSyntax)ctx.Node);
            //var dontOverrideAttributes = context.SyntaxProvider.CreateSyntaxProvider(
            //    (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDontOverrideAttribute(@as),
            //    (ctx, ct) => (AttributeSyntax)ctx.Node);
            //var arrangeInvocations = context.SyntaxProvider.CreateSyntaxProvider(
            //    (sn, ct) => sn is InvocationExpressionSyntax ies && LightMockSyntaxReceiver.IsArrangeInvocation(ies),
            //    (ctx, ct) => (InvocationExpressionSyntax)ctx.Node);
            //candidateMocks.Collect().Combine(disableCodegenerationAttributes.Collect()).
        }

        INamedTypeSymbol? ConvertToInterface(GeneratorSyntaxContext context)
        {
            GenericNameSyntax candidateGeneric;
            var semanticModel = context.SemanticModel;

            switch (context.Node)
            {
                case ObjectCreationExpressionSyntax { Type: GenericNameSyntax gns }:
                    candidateGeneric = gns;
                    break;
                case ObjectCreationExpressionSyntax { Type: QualifiedNameSyntax { Right: GenericNameSyntax gns } }:
                    candidateGeneric = gns;
                    break;
                default:
                    return null;
            }

            var mockContainer = semanticModel.GetSymbolInfo(candidateGeneric).Symbol
                as INamedTypeSymbol;
            var mcbt = mockContainer?.BaseType;
            if (mcbt != null
                && mockContextMatcher.IsMatch(mcbt)
                && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType)
            {

                var mtbt = mockedType.BaseType;
                if (mtbt == null)
                    return mockedType;
            }
            return null;
        }


#else

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new LightMockSyntaxReceiver());
        }
#endif

    }
}
