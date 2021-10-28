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

        static bool IsGenerationDisabledByOptions(AnalyzerConfigOptionsProvider optionsProvider)
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
            var disableCodegenerationAttributes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDisableCodeGenerationAttribute(@as),
                (ctx, ct) => LightMockSyntaxReceiver.IsDisableCodeGenerationAttribute(ctx.SemanticModel, (AttributeSyntax)ctx.Node));

            var interfaces = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => IsMock(sn),
                (ctx, ct) => ConvertToInterface(ctx));
            context.RegisterSourceOutput(interfaces
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Select((comb, ct) => (candidate: comb.Left.Left, compilation: comb.Left.Right, options: comb.Right))
                .Combine(disableCodegenerationAttributes.Collect())
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, disableCodegenerationAttributes: comb.Right))
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, comb.Left.disableCodegenerationAttributes, parseOptions: comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false 
                && t.candidate != null
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp, (CSharpCompilation)sr.compilation, (CSharpParseOptions)sr.parseOptions),
                    new InterfaceProcessor(sr.candidate!),
                    sp.CancellationToken));

            var delegates = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => IsMock(sn),
                (ctx, ct) => ConvertToDelegate(ctx));
            context.RegisterSourceOutput(delegates
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Select((comb, ct) => (candidate: comb.Left.Left, compilation: comb.Left.Right, options: comb.Right))
                .Combine(disableCodegenerationAttributes.Collect())
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, disableCodegenerationAttributes: comb.Right))
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, comb.Left.disableCodegenerationAttributes, parseOptions: comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false
                && t.candidate != null
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp, (CSharpCompilation)sr.compilation, (CSharpParseOptions)sr.parseOptions),
                    new DelegateProcessor(sr.candidate!),
                    sp.CancellationToken));

            var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => IsMock(sn),
                (ctx, ct) => ConvertToAbstractClass(ctx));
            var dontOverrideTypes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDontOverrideAttribute(@as),
                (ctx, ct) => CovertToDontOverride(ctx.SemanticModel, (AttributeSyntax)ctx.Node));
            context.RegisterSourceOutput(classes
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Select((comb, ct) => (candidate: comb.Left.Left, compilation: comb.Left.Right, options: comb.Right))
                .Combine(disableCodegenerationAttributes.Collect())
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, disableCodegenerationAttributes: comb.Right))
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, comb.Left.disableCodegenerationAttributes, parseOptions: comb.Right))
                .Combine(dontOverrideTypes.Collect())
                .Select((comb, ct) => (comb.Left.candidate, comb.Left.compilation, comb.Left.options, comb.Left.disableCodegenerationAttributes, comb.Left.parseOptions, dontOverrideTypes: comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false
                && t.candidate != null
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp, (CSharpCompilation)sr.compilation, (CSharpParseOptions)sr.parseOptions),
                    new AbstractClassProcessor(sr.candidate!.Value.mock, sr.candidate!.Value.mockedType, sr.dontOverrideTypes),
                    sp.CancellationToken));
        }

        bool IsMock(SyntaxNode node)
        {
            var gns = GetMockSymbol(node);
            return gns != null && LightMockSyntaxReceiver.IsMock(gns);
        }

        GenericNameSyntax? GetMockSymbol(SyntaxNode node)
        {
            switch (node)
            {
                case ObjectCreationExpressionSyntax { Type: GenericNameSyntax gns }:
                    return gns;
                case ObjectCreationExpressionSyntax { Type: QualifiedNameSyntax { Right: GenericNameSyntax gns } }:
                    return gns;
            }
            return null;
        }

        static bool IsCodeGenerationDisabledByAttributes(ImmutableArray<bool> attributes)
            => attributes.Where(t => t == true).Any() == false;

        INamedTypeSymbol? ConvertToInterface(GeneratorSyntaxContext context)
        {
            var candidateGeneric = GetMockSymbol(context.Node);
            var semanticModel = context.SemanticModel;

            if (candidateGeneric != null)
            {
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
            }
            return null;
        }

        INamedTypeSymbol? ConvertToDelegate(GeneratorSyntaxContext context)
        {
            var candidateGeneric = GetMockSymbol(context.Node);
            var semanticModel = context.SemanticModel;

            if (candidateGeneric != null)
            {
                var mockContainer = semanticModel.GetSymbolInfo(candidateGeneric).Symbol
                    as INamedTypeSymbol;
                var mcbt = mockContainer?.BaseType;
                if (mcbt != null
                    && mockContextMatcher.IsMatch(mcbt)
                    && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType)
                {

                    var mtbt = mockedType.BaseType;
                    if (mtbt != null && mtbt.ToDisplayString(SymbolDisplayFormats.Namespace) == multicastDelegateNameSpaceAndName)
                        return mockedType;
                }
            }
            return null;
        }

        (GenericNameSyntax mock, INamedTypeSymbol mockedType)? ConvertToAbstractClass(GeneratorSyntaxContext context)
        {
            var candidateGeneric = GetMockSymbol(context.Node);
            var semanticModel = context.SemanticModel;

            if (candidateGeneric != null)
            {
                var mockContainer = semanticModel.GetSymbolInfo(candidateGeneric).Symbol
                    as INamedTypeSymbol;
                var mcbt = mockContainer?.BaseType;
                if (mcbt != null
                    && mockContextMatcher.IsMatch(mcbt)
                    && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType)
                {

                    var mtbt = mockedType.BaseType;
                    if (mtbt != null)
                    {
                        if (mtbt.ToDisplayString(SymbolDisplayFormats.Namespace) != multicastDelegateNameSpaceAndName)
                            return (candidateGeneric, mockedType);
                    }
                }
            }
            return null;
        }

        INamedTypeSymbol? CovertToDontOverride(SemanticModel semanticModel, AttributeSyntax @as)
        {
            var dontOverrideAttributeType = typeof(DontOverrideAttribute);
            var doatName = dontOverrideAttributeType.Name;
            var doatNamespace = dontOverrideAttributeType.Namespace;
            TypeSyntax? type;
            if (semanticModel.GetSymbolInfo(@as).Symbol is IMethodSymbol methodSymbol
                && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == doatName
                && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == doatNamespace
                && (type = TypeOfLocator.Locate(@as)?.Type) != null
                && semanticModel.GetSymbolInfo(type).Symbol is INamedTypeSymbol typeSymbol)
            {
                return typeSymbol;
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
