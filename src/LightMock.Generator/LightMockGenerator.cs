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
        const string KMock = "Mock";

        readonly Lazy<SourceText> mock = new(
            () => SourceText.From(Utils.LoadResource(KMock + Suffix.CSharpFile), Encoding.UTF8));

        public LightMockGenerator()
        {
        }

#if ROSLYN_4 == false

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation is CSharpCompilation compilation == false)
                return;
            if (context.SyntaxContextReceiver is LightMockSyntaxReceiver receiver == false)
                return;
            DoGenerate(
                context,
                static (context, diagnostic) => context.ReportDiagnostic(diagnostic),
                static (context, hintName, sourceText) => context.AddSource(hintName, sourceText),
                compilation,
                context.AnalyzerConfigOptions,
                receiver.CandidateMocks.ToImmutableArray(),
                receiver.DisableCodeGenerationAttributes.ToImmutableArray(),
                receiver.DontOverrideAttributes.ToImmutableArray(),
                receiver.ArrangeInvocations.ToImmutableArray(),
                context.CancellationToken);
        }

#endif
        public void DoGenerate<TContext>(
            TContext context,
            Action<TContext, Diagnostic> reportDiagnostic,
            Action<TContext, string, SourceText> addSource,
            CSharpCompilation compilation,
            AnalyzerConfigOptionsProvider optionsProvider,
            ImmutableArray<GenericNameSyntax> candidateMocks,
            ImmutableArray<AttributeSyntax> disableCodeGenerationAttributes,
            ImmutableArray<AttributeSyntax> dontOverrideAttributes,
            ImmutableArray<InvocationExpressionSyntax> arrangeInvocations,
            CancellationToken cancellationToken)
        { 
            cancellationToken.ThrowIfCancellationRequested();
            if (optionsProvider.GlobalOptions.TryGetValue(GlobalOptionsNames.Enable, out var value)
                && value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (compilation.SyntaxTrees.First().Options is CSharpParseOptions options)
            {
                if (IsDisableCodeGenerationAttributePresent(compilation, disableCodeGenerationAttributes, cancellationToken))
                    return;

                var dontOverrideList = GetClassExclusionList(compilation, dontOverrideAttributes, cancellationToken);

                //addSource(context, KMock + Suffix.FileName, mock.Value);

                //compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                //    mock.Value, options, cancellationToken: cancellationToken));

                // process symbols under Mock<> generic

                var mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
                var multicastDelegateType = typeof(MulticastDelegate);
                var multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;

                foreach (var candidateGeneric in candidateMocks)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var mockContainer = compilation
                        .GetSemanticModel(candidateGeneric.SyntaxTree)
                        .GetSymbolInfo(candidateGeneric, cancellationToken).Symbol
                        as INamedTypeSymbol;
                    cancellationToken.ThrowIfCancellationRequested();
                    var mcbt = mockContainer?.BaseType;
                    if (mcbt != null
                        && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType
                        )
                    {
                        ClassProcessor processor;
                        var mtbt = mockedType.BaseType;
                        if (mtbt != null)
                        {
                            if (mtbt.ToDisplayString(SymbolDisplayFormats.Namespace) == multicastDelegateNameSpaceAndName)
                                processor = new DelegateProcessor(mockedType);
                            else
                                processor = new AbstractClassProcessor(candidateGeneric, mockedType, dontOverrideList);
                        }
                        else
                            processor = new InterfaceProcessor(mockedType);

                        cancellationToken.ThrowIfCancellationRequested();
                        if (EmitDiagnostics(context, reportDiagnostic, processor.GetErrors()))
                            continue;
                        cancellationToken.ThrowIfCancellationRequested();
                        EmitDiagnostics(context, reportDiagnostic, processor.GetWarnings());
                        var text = processor.DoGenerate();
                        addSource(context, processor.FileName, text);
                        if (processor.IsUpdateCompilationRequired)
                        {
                            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                                text, options, cancellationToken: cancellationToken));
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                // process symbols under ArrangeSetter
                
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
            }
        }

        private static bool IsDisableCodeGenerationAttributePresent(
            CSharpCompilation compilation,
            ImmutableArray<AttributeSyntax> disableCodeGenerationAttributes,
            CancellationToken cancellationToken)
        {
            var disableCodeGenerationAttributeType = typeof(DisableCodeGenerationAttribute);
            var dcgaName = disableCodeGenerationAttributeType.Name;
            var dcgaNamespace = disableCodeGenerationAttributeType.Namespace;
            foreach (var candidateAttribute in disableCodeGenerationAttributes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var model = compilation.GetSemanticModel(candidateAttribute.SyntaxTree);
                var si = model.GetSymbolInfo(candidateAttribute, cancellationToken);
                if (si.Symbol is IMethodSymbol methodSymbol
                    && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == dcgaName
                    && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == dcgaNamespace)
                {
                    return true;
                }
            }
            return false;
        }

        private static IReadOnlyList<INamedTypeSymbol> GetClassExclusionList(
            CSharpCompilation compilation,
            ImmutableArray<AttributeSyntax> dontOverrideAttributes,
            CancellationToken cancellationToken)
        {
            var result = new List<INamedTypeSymbol>();
            var dontOverrideAttributeType = typeof(DontOverrideAttribute);
            var doatName = dontOverrideAttributeType.Name;
            var doatNamespace = dontOverrideAttributeType.Namespace;
            foreach (var candidateAttribute in dontOverrideAttributes)
            {
                TypeSyntax? type;
                cancellationToken.ThrowIfCancellationRequested();
                var sm = compilation.GetSemanticModel(candidateAttribute.SyntaxTree);
                if (sm.GetSymbolInfo(candidateAttribute, cancellationToken).Symbol is IMethodSymbol methodSymbol
                    && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == doatName
                    && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == doatNamespace
                    && (type = TypeOfLocator.Locate(candidateAttribute)?.Type) != null
                    && sm.GetSymbolInfo(type, cancellationToken).Symbol is INamedTypeSymbol typeSymbol)
                {
                    result.Add(typeSymbol);
                }
            }
            return result.ToImmutableArray();
        }

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
            var candidateMocks = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is GenericNameSyntax gns && LightMockSyntaxReceiver.IsMock(gns),
                (ctx, ct) => (GenericNameSyntax)ctx.Node);
            var disableCodegenerationAttributes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDisableCodeGenerationAttribute(@as),
                (ctx, ct) => (AttributeSyntax)ctx.Node);
            var dontOverrideAttributes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && LightMockSyntaxReceiver.IsDontOverrideAttribute(@as),
                (ctx, ct) => (AttributeSyntax)ctx.Node);
            var arrangeInvocations = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is InvocationExpressionSyntax ies && LightMockSyntaxReceiver.IsArrangeInvocation(ies),
                (ctx, ct) => (InvocationExpressionSyntax)ctx.Node);
            //candidateMocks.Collect().Combine(disableCodegenerationAttributes.Collect()).
        }

#else

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new LightMockSyntaxReceiver());
            context.RegisterForPostInitialization((gpic) => 
            {
                gpic.AddSource(KMock + Suffix.FileName, mock.Value);
            });
        }
#endif

    }
}
