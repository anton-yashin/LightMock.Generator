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
        readonly Lazy<SourceText> contextResolver = new(
            () => SourceText.From(Utils.LoadResource(KContextResolver + Suffix.CSharpFile), Encoding.UTF8));

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
                if (IsDisableCodeGenerationAttributePresent(compilation, receiver))
                    return;

                var dontOverrideList = GetClassExclusionList(compilation, receiver);

                context.AddSource(KMock + Suffix.FileName, mock.Value);
                context.AddSource(KContextResolver + Suffix.FileName, contextResolver.Value);

                compilation = compilation
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(mock.Value, options))
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(contextResolver.Value, options));

                // process symbols under Mock<> generic

                var mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
                var getInstanceTypeBuilder = new StringBuilder();
                var getProtectedContextTypeBuilder = new StringBuilder();
                var getPropertiesContextTypeBuilder = new StringBuilder();
                var getAssertTypeBuilder = new StringBuilder();
                var getDelegateBuilder = new StringBuilder();
                var exchangeForExpressionBuilder = new StringBuilder();
                var processedTypes = new List<INamedTypeSymbol>();
                var multicastDelegateType = typeof(MulticastDelegate);
                var multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;

                foreach (var candidateGeneric in receiver.CandidateMocks)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var mockContainer = compilation
                        .GetSemanticModel(candidateGeneric.SyntaxTree)
                        .GetSymbolInfo(candidateGeneric).Symbol
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
                        processor.DoGeneratePart_GetInstanceType(getInstanceTypeBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processor.DoGeneratePart_GetProtectedContextType(getProtectedContextTypeBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processor.DoGeneratePart_GetPropertiesContextType(getPropertiesContextTypeBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processor.DoGeneratePart_GetAssertType(getAssertTypeBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processor.DoGeneratePart_GetDelegate(getDelegateBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processor.DoGeneratePart_ExchangeForExpression(exchangeForExpressionBuilder);
                        context.CancellationToken.ThrowIfCancellationRequested();
                        processedTypes.Add(mockedType.OriginalDefinition);
                    }
                }

                var expressionUids = new HashSet<string>();
                foreach (var candidateInvocation in receiver.ArrangeInvocations)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var methodSymbol = compilation.GetSemanticModel(candidateInvocation.SyntaxTree)
                        .GetSymbolInfo(candidateInvocation).Symbol as IMethodSymbol;
                    context.CancellationToken.ThrowIfCancellationRequested();

                    if (methodSymbol != null 
                        && methodSymbol.Name == nameof(AbstractMockNameofProvider.ArrangeSetter)
                        && mockContextMatcher.IsMatch(methodSymbol.ContainingType))
                    {
                        var processor = new ExpressionRewirter(methodSymbol, candidateInvocation, compilation, expressionUids);

                        context.CancellationToken.ThrowIfCancellationRequested();
                        if (EmitDiagnostics(context, processor.GetErrors()))
                            continue;
                        context.CancellationToken.ThrowIfCancellationRequested();
                        EmitDiagnostics(context, processor.GetWarnings());

                        processor.AppendExpression(exchangeForExpressionBuilder);
                    }
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                var impl = Utils.LoadResource(KContextResolver + Suffix.ImplFile + Suffix.CSharpFile)
                    .Replace("/*getInstanceTypeBuilder*/", getInstanceTypeBuilder.ToString())
                    .Replace("/*getProtectedContextTypeBuilder*/", getProtectedContextTypeBuilder.ToString())
                    .Replace("/*getPropertiesContextTypeBuilder*/", getPropertiesContextTypeBuilder.ToString())
                    .Replace("/*getAssertTypeBuilder*/", getAssertTypeBuilder.ToString())
                    .Replace("/*getDelegateBuilder*/", getDelegateBuilder.ToString())
                    .Replace("/*exchangeForExpressionBuilder*/", exchangeForExpressionBuilder.ToString());

                context.CancellationToken.ThrowIfCancellationRequested();
                context.AddSource(KContextResolver + Suffix.ImplFile + Suffix.FileName, SourceText.From(impl, Encoding.UTF8));
            }
        }

        private static bool IsDisableCodeGenerationAttributePresent(CSharpCompilation compilation, LightMockSyntaxReceiver receiver)
        {
            var disableCodeGenerationAttributeType = typeof(DisableCodeGenerationAttribute);
            var dcgaName = disableCodeGenerationAttributeType.Name;
            var dcgaNamespace = disableCodeGenerationAttributeType.Namespace;
            foreach (var candidateAttribute in receiver.DisableCodeGenerationAttributes)
            {
                var model = compilation.GetSemanticModel(candidateAttribute.SyntaxTree);
                var si = model.GetSymbolInfo(candidateAttribute);
                if (si.Symbol is IMethodSymbol methodSymbol
                    && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == dcgaName
                    && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == dcgaNamespace)
                {
                    return true;
                }
            }
            return false;
        }

        private static IReadOnlyList<INamedTypeSymbol> GetClassExclusionList(CSharpCompilation compilation, LightMockSyntaxReceiver receiver)
        {
            var result = new List<INamedTypeSymbol>();
            var dontOverrideAttributeType = typeof(DontOverrideAttribute);
            var doatName = dontOverrideAttributeType.Name;
            var doatNamespace = dontOverrideAttributeType.Namespace;
            foreach (var candidateAttribute in receiver.DontOverrideAttributes)
            {
                TypeSyntax? type;
                var sm = compilation.GetSemanticModel(candidateAttribute.SyntaxTree);
                if (sm.GetSymbolInfo(candidateAttribute).Symbol is IMethodSymbol methodSymbol
                    && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == doatName
                    && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == doatNamespace
                    && (type = ExclusionTypeFinder.FindAt(candidateAttribute)) != null
                    && sm.GetSymbolInfo(type).Symbol is INamedTypeSymbol typeSymbol)
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
            context.CancellationToken.ThrowIfCancellationRequested();
            context.RegisterForSyntaxNotifications(() => new LightMockSyntaxReceiver(context.CancellationToken));
        }
    }
}
