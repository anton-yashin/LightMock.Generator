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
            if (IsCompilationDisabledByOptions(context.AnalyzerConfigOptions) || receiver.DisableCodeGeneration)
                return;
            compilation = DoGenerateAbstractClasses(
                context,
                ContextReportDiagnostic,
                ContextAddSource,
                compilation,
                parseOptions,
                receiver.AbstractClasses.ToImmutableArray(),
                receiver.DontOverrideTypes.ToImmutableArray(),
                context.CancellationToken);
            compilation = DoGenerateInterfaces(
                context,
                ContextReportDiagnostic,
                ContextAddSource,
                compilation,
                parseOptions,
                receiver.Interfaces.ToImmutableArray(),
                context.CancellationToken);
            compilation = DoGenerateDelegates(
                context,
                ContextReportDiagnostic,
                ContextAddSource,
                compilation,
                parseOptions,
                receiver.Delegates.ToImmutableArray(),
                context.CancellationToken);
            compilation = DoGenerate(
                context,
                ContextReportDiagnostic,
                ContextAddSource,
                compilation,
                parseOptions,
                receiver.ArrangeInvocations.ToImmutableArray(),
                context.CancellationToken);
        }

        static void ContextReportDiagnostic(GeneratorExecutionContext context, Diagnostic diagnostic)
            => context.ReportDiagnostic(diagnostic);
        static void ContextAddSource(GeneratorExecutionContext context, string hintName, SourceText sourceText)
            => context.AddSource(hintName, sourceText);

#endif
        public CSharpCompilation DoGenerate<TContext>(
            TContext context,
            Action<TContext, Diagnostic> reportDiagnostic,
            Action<TContext, string, SourceText> addSource,
            CSharpCompilation compilation,
            CSharpParseOptions parseOptions,
            ImmutableArray<InvocationExpressionSyntax> arrangeInvocations,
            CancellationToken cancellationToken)
            where TContext : struct
        {
            if (reportDiagnostic is null)
                throw new ArgumentNullException(nameof(reportDiagnostic));
            if (addSource is null)
                throw new ArgumentNullException(nameof(addSource));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));
            if (parseOptions is null)
                throw new ArgumentNullException(nameof(parseOptions));

            cancellationToken.ThrowIfCancellationRequested();

            // process symbols under ArrangeSetter

            var mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            var processedFiles = new HashSet<string>();
            var mockInterfaceMatcher = new TypeMatcher(typeof(IAdvancedMockContext<>));
            foreach (var candidateInvocation in arrangeInvocations)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var st = compilation.GetSemanticModel(candidateInvocation.SyntaxTree);
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
                            processor = new ArrangeExpressionRewriter(methodSymbol, candidateInvocation, compilation);
                            break;
                        case nameof(AbstractMockNameofProvider.AssertSet):
                            processor = new AssertExpressionRewriter(methodSymbol, candidateInvocation, compilation);
                            break;
                        default:
                            continue;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    if (processedFiles.Contains(processor.FileName))
                    {
                        reportDiagnostic(context, Diagnostic.Create(
                            DiagnosticsDescriptors.KPropertyExpressionMustHaveUniqueId,
                            candidateInvocation.GetLocation(), methodSymbol.Name));
                        continue;
                    }
                    if (EmitDiagnostics(context, reportDiagnostic, processor.GetErrors()))
                        continue;
                    cancellationToken.ThrowIfCancellationRequested();
                    EmitDiagnostics(context, reportDiagnostic, processor.GetWarnings());
                    cancellationToken.ThrowIfCancellationRequested();
                    var text = processor.DoGenerate();
                    addSource(context, processor.FileName, text);
                }
            }
            return compilation;
        }

        public CSharpCompilation DoGenerateDelegates<TContext>(
            TContext context,
            Action<TContext, Diagnostic> reportDiagnostic,
            Action<TContext, string, SourceText> addSource,
            CSharpCompilation compilation,
            CSharpParseOptions parseOptions,
            ImmutableArray<INamedTypeSymbol> delegates,
            CancellationToken cancellationToken)
            where TContext : struct
        {
            if (reportDiagnostic is null)
                throw new ArgumentNullException(nameof(reportDiagnostic));
            if (addSource is null)
                throw new ArgumentNullException(nameof(addSource));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));
            if (parseOptions is null)
                throw new ArgumentNullException(nameof(parseOptions));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var @delegate in delegates)
            {
                compilation = DoGenerateCode(
                    context,
                    reportDiagnostic,
                    addSource,
                    compilation,
                    parseOptions,
                    new DelegateProcessor(@delegate),
                    cancellationToken);
            }
            return compilation;
        }


        public CSharpCompilation DoGenerateAbstractClasses<TContext>(
            TContext context,
            Action<TContext, Diagnostic> reportDiagnostic,
            Action<TContext, string, SourceText> addSource,
            CSharpCompilation compilation,
            CSharpParseOptions parseOptions,
            ImmutableArray<(GenericNameSyntax mock, INamedTypeSymbol mockedType)> abstractClasses,
            ImmutableArray<INamedTypeSymbol> dontOverrideTypes,
            CancellationToken cancellationToken)
            where TContext : struct
        {
            if (reportDiagnostic is null)
                throw new ArgumentNullException(nameof(reportDiagnostic));
            if (addSource is null)
                throw new ArgumentNullException(nameof(addSource));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));
            if (parseOptions is null)
                throw new ArgumentNullException(nameof(parseOptions));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var (candidateGeneric, mockedType) in abstractClasses)
            {
                compilation = DoGenerateCode(
                    context,
                    reportDiagnostic,
                    addSource,
                    compilation,
                    parseOptions,
                    new AbstractClassProcessor(candidateGeneric, mockedType, dontOverrideTypes),
                    cancellationToken);
            }
            return compilation;
        }

        public CSharpCompilation DoGenerateInterfaces<TContext>(
            TContext context,
            Action<TContext, Diagnostic> reportDiagnostic,
            Action<TContext, string, SourceText> addSource,
            CSharpCompilation compilation,
            CSharpParseOptions parseOptions,
            ImmutableArray<INamedTypeSymbol> interfaces,
            CancellationToken cancellationToken)
            where TContext : struct
        {
            if (reportDiagnostic is null)
                throw new ArgumentNullException(nameof(reportDiagnostic));
            if (addSource is null)
                throw new ArgumentNullException(nameof(addSource));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));
            if (parseOptions is null)
                throw new ArgumentNullException(nameof(parseOptions));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var @interface in interfaces)
            {
                compilation = DoGenerateCode(
                    context,
                    reportDiagnostic,
                    addSource,
                    compilation,
                    parseOptions,
                    new InterfaceProcessor(@interface),
                    cancellationToken);
            }
            return compilation;
        }

        CSharpCompilation DoGenerateCode<TContext>(
            TContext context,
            Action<TContext, Diagnostic> reportDiagnostic,
            Action<TContext, string, SourceText> addSource,
            CSharpCompilation compilation,
            CSharpParseOptions parseOptions,
            ClassProcessor classProcessor,
            CancellationToken cancellationToken)
            where TContext : struct
        {
            if (reportDiagnostic is null)
                throw new ArgumentNullException(nameof(reportDiagnostic));
            if (addSource is null)
                throw new ArgumentNullException(nameof(addSource));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));
            if (parseOptions is null)
                throw new ArgumentNullException(nameof(parseOptions));
            if (classProcessor is null)
                throw new ArgumentNullException(nameof(classProcessor));

            cancellationToken.ThrowIfCancellationRequested();

            cancellationToken.ThrowIfCancellationRequested();
            if (EmitDiagnostics(context, reportDiagnostic, classProcessor.GetErrors()))
                return compilation;
            cancellationToken.ThrowIfCancellationRequested();
            EmitDiagnostics(context, reportDiagnostic, classProcessor.GetWarnings());
            var text = classProcessor.DoGenerate();
            addSource(context, classProcessor.FileName, text);
            if (classProcessor.IsUpdateCompilationRequired)
            {
                compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                    text, parseOptions, cancellationToken: cancellationToken));
            }
            cancellationToken.ThrowIfCancellationRequested();
            return compilation;
        }



        bool IsCompilationDisabledByOptions(AnalyzerConfigOptionsProvider optionsProvider)
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
                && t.candidate != null && IsCompilationDisabledByOptions(t.options) == false
                && t.compilation is CSharpCompilation && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(sp, 
                static (ctx, diag) => ctx.ReportDiagnostic(diag),
                static (ctx, hint, text) => ctx.AddSource(hint, text),
                (CSharpCompilation)sr.compilation,
                (CSharpParseOptions)sr.parseOptions,
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
