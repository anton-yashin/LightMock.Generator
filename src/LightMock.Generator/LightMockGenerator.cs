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
using Microsoft.CodeAnalysis.Text;

namespace LightMock.Generator
{
    [Generator]
    public sealed class LightMockGenerator : ISourceGenerator
    {
        const string KMock = "Mock";
        const string KContextResolver = nameof(ContextResolver);

        readonly Lazy<SourceText> mock = new(
            () => SourceText.From(Utils.LoadResource(KMock + Suffix.CSharpFile), Encoding.UTF8));

        public LightMockGenerator()
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(GlobalOptionsNames.Enable, out var value)
                && value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (context.Compilation is CSharpCompilation compilation &&
                context.SyntaxReceiver is LightMockSyntaxReceiver receiver &&
                compilation.SyntaxTrees.First().Options is CSharpParseOptions options)
            {
                if (IsDisableCodeGenerationAttributePresent(compilation, receiver, context.CancellationToken))
                    return;

                var dontOverrideList = GetClassExclusionList(compilation, receiver, context.CancellationToken);

                context.AddSource(KMock + Suffix.FileName, mock.Value);

                compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                    mock.Value, options, cancellationToken: context.CancellationToken));

                // process symbols under Mock<> generic

                var mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
                var typeByTypeBuilder = new StringBuilder();
                var exchangeForExpressionBuilder = new StringBuilder();
                var processedTypes = new List<INamedTypeSymbol>();
                var multicastDelegateType = typeof(MulticastDelegate);
                var multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;

                foreach (var candidateGeneric in receiver.CandidateMocks)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var mockContainer = compilation
                        .GetSemanticModel(candidateGeneric.SyntaxTree)
                        .GetSymbolInfo(candidateGeneric, context.CancellationToken).Symbol
                        as INamedTypeSymbol;
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var mcbt = mockContainer?.BaseType;
                    if (mcbt != null
                        && mockContextMatcher.IsMatch(mcbt)
                        && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType
                        && processedTypes.Contains(mockedType.OriginalDefinition) == false)
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

                        context.CancellationToken.ThrowIfCancellationRequested();
                        if (EmitDiagnostics(context, processor.GetErrors()))
                            continue;
                        context.CancellationToken.ThrowIfCancellationRequested();
                        EmitDiagnostics(context, processor.GetWarnings());
                        context.AddSource(processor.FileName, processor.DoGenerate());
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processor.DoGeneratePart_TypeByType(typeByTypeBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processedTypes.Add(mockedType.OriginalDefinition);
                    }
                }

                // process symbols under ArrangeSetter
                
                var expressionUids = new HashSet<string>();
                var mockInterfaceMatcher = new TypeMatcher(typeof(IMock<>));
                foreach (var candidateInvocation in receiver.ArrangeInvocations)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var methodSymbol = compilation.GetSemanticModel(candidateInvocation.SyntaxTree)
                        .GetSymbolInfo(candidateInvocation, context.CancellationToken).Symbol as IMethodSymbol;
                    context.CancellationToken.ThrowIfCancellationRequested();

                    if (methodSymbol != null 
                        && (mockContextMatcher.IsMatch(methodSymbol.ContainingType)
                            || mockInterfaceMatcher.IsMatch(methodSymbol.ContainingType)))
                    {
                        ExpressionRewriter processor;
                        switch (methodSymbol.Name)
                        {
                            case nameof(AbstractMockNameofProvider.ArrangeSetter):
                                processor = new ArrangeExpressionRewriter(methodSymbol, candidateInvocation, compilation, expressionUids);
                                break;
                            case nameof(AbstractMockNameofProvider.AssertSet):
                                processor = new AssertExpressionRewriter(methodSymbol, candidateInvocation, compilation, expressionUids);
                                break;
                            default:
                                continue;
                        }

                        context.CancellationToken.ThrowIfCancellationRequested();

                        processor.AppendExpression(exchangeForExpressionBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        if (EmitDiagnostics(context, processor.GetErrors()))
                            continue;
                        context.CancellationToken.ThrowIfCancellationRequested();
                        EmitDiagnostics(context, processor.GetWarnings());

                    }
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                var impl = Utils.LoadResource(KContextResolver + Suffix.CSharpFile)
                    .Replace("/*typeByTypeBuilder*/", typeByTypeBuilder.ToString())
                    .Replace("/*exchangeForExpressionBuilder*/", exchangeForExpressionBuilder.ToString());

                context.CancellationToken.ThrowIfCancellationRequested();
                context.AddSource(KContextResolver + Suffix.FileName, SourceText.From(impl, Encoding.UTF8));
            }
        }

        private static bool IsDisableCodeGenerationAttributePresent(
            CSharpCompilation compilation,
            LightMockSyntaxReceiver receiver,
            CancellationToken cancellationToken)
        {
            var disableCodeGenerationAttributeType = typeof(DisableCodeGenerationAttribute);
            var dcgaName = disableCodeGenerationAttributeType.Name;
            var dcgaNamespace = disableCodeGenerationAttributeType.Namespace;
            foreach (var candidateAttribute in receiver.DisableCodeGenerationAttributes)
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
            LightMockSyntaxReceiver receiver,
            CancellationToken cancellationToken)
        {
            var result = new List<INamedTypeSymbol>();
            var dontOverrideAttributeType = typeof(DontOverrideAttribute);
            var doatName = dontOverrideAttributeType.Name;
            var doatNamespace = dontOverrideAttributeType.Namespace;
            foreach (var candidateAttribute in receiver.DontOverrideAttributes)
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

        bool EmitDiagnostics(GeneratorExecutionContext context, IEnumerable<Diagnostic> diagnostics)
        {
            bool haveIssues = false;
            foreach (var d in diagnostics)
            {
                haveIssues = true;
                context.ReportDiagnostic(d);
            }
            return haveIssues;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new LightMockSyntaxReceiver());
        }
    }
}
