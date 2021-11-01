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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly string doatName;
        private readonly string doatNamespace;
        private readonly ConditionalWeakTable<Compilation, CompilationContext> compilationContexts;
        private readonly SyntaxHelpers syntaxHelpers;

        public LightMockGenerator()
        {
            mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            mockInterfaceMatcher = new TypeMatcher(typeof(IAdvancedMockContext<>));
            var multicastDelegateType = typeof(MulticastDelegate);
            multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;
            var dontOverrideAttributeType = typeof(DontOverrideAttribute);
            doatName = dontOverrideAttributeType.Name;
            doatNamespace = dontOverrideAttributeType.Namespace;
            compilationContexts = new ConditionalWeakTable<Compilation, CompilationContext>();
            syntaxHelpers = new SyntaxHelpers();
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

            var cc = new CodeGenerationContext(context, compilation, parseOptions, GetCompilationContext(compilation));

            DoGenerateCode(
                cc,
                receiver.AbstractClasses.Select(
                    t => new AbstractClassProcessor(
                        t.mock, t.mockedType, receiver.DontOverrideTypes)),
                context.CancellationToken);
            DoGenerateCode(
                cc,
                receiver.Interfaces.Select(t => new InterfaceProcessor(t)),
                context.CancellationToken);
            DoGenerateCode(
                cc,
                receiver.Delegates.Select(t => new DelegateProcessor(t)),
                context.CancellationToken);
            DoGenerateInvocations(
                cc,
                receiver.CandidateInvocations.ToImmutableArray(),
                context.CancellationToken);
        }

#endif

        void DoGenerateInvocations(
            CodeGenerationContext context,
            ImmutableArray<CandidateInvocation> candidateInvocations,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var candidate in candidateInvocations)
                DoGenerateInvocation(context, candidate, cancellationToken);
        }

        void DoGenerateInvocation(
            CodeGenerationContext context,
            CandidateInvocation candidate,
            CancellationToken cancellationToken)
        {
            var (methodSymbol, candidateInvocation, node) = candidate;
            if (methodSymbol == null || candidateInvocation == null)
            {
                context = context.UpdateFromCompilationContext();
                (methodSymbol, candidateInvocation, node) = syntaxHelpers.ConvertToInvocation(
                    node, context.Compilation.GetSemanticModel(node.SyntaxTree), cancellationToken);
                if (methodSymbol == null || candidateInvocation == null)
                    return;
            }
            DoGenerateInvocation(context, methodSymbol, candidateInvocation, cancellationToken);
        }

        private static void DoGenerateInvocation(
            CodeGenerationContext context,
            IMethodSymbol methodSymbol,
            InvocationExpressionSyntax candidateInvocation,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                    return;
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (context.CompilationContext.IsTagExits(processor.FileName))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticsDescriptors.KPropertyExpressionMustHaveUniqueId,
                    candidateInvocation.GetLocation(), methodSymbol.Name));
                return;
            }
            if (context.EmitDiagnostics(processor.GetErrors()))
                return;
            cancellationToken.ThrowIfCancellationRequested();
            context.EmitDiagnostics(processor.GetWarnings());
            cancellationToken.ThrowIfCancellationRequested();
            var text = processor.DoGenerate();
            context.AddSource(processor.FileName, text);
            context.CompilationContext.AddTag(processor.FileName);
        }

        void DoGenerateCode(
            CodeGenerationContext context,
            IEnumerable<ClassProcessor> classProcessors,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var classProcessor in classProcessors)
                DoGenerateCode(context, classProcessor, cancellationToken);
        }

        void DoGenerateCode(
            CodeGenerationContext context,
            ClassProcessor classProcessor,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (context.EmitDiagnostics(classProcessor.GetErrors()))
                return;
            cancellationToken.ThrowIfCancellationRequested();
            context.EmitDiagnostics(classProcessor.GetWarnings());
            var text = classProcessor.DoGenerate();
            if (context.CompilationContext.IsTagExits(classProcessor.FileName) == false)
            {
                context.AddSource(classProcessor.FileName, text);
                context.CompilationContext.AddTag(classProcessor.FileName);
                if (classProcessor.IsUpdateCompilationRequired)
                {
                    context.CompilationContext.AddSyntaxTree(CSharpSyntaxTree.ParseText(
                        text, context.ParseOptions, cancellationToken: cancellationToken));
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        static bool IsGenerationDisabledByOptions(AnalyzerConfigOptionsProvider optionsProvider)
            => optionsProvider.GlobalOptions.TryGetValue(GlobalOptionsNames.Enable, out var value)
                && value.Equals("false", StringComparison.InvariantCultureIgnoreCase);

        CompilationContext GetCompilationContext(Compilation compilation)
        {
            lock (compilationContexts)
                return compilationContexts.GetOrCreateValue(compilation);
        }

#if ROSLYN_4

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var disableCodegenerationAttributes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && SyntaxHelpers.IsDisableCodeGenerationAttribute(@as),
                (ctx, ct) => syntaxHelpers.IsDisableCodeGenerationAttribute(ctx.SemanticModel, (AttributeSyntax)ctx.Node));

            var interfaces = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => SyntaxHelpers.IsMock(sn),
                (ctx, ct) => ConvertToInterface(ctx));
            context.RegisterSourceOutput(interfaces
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(disableCodegenerationAttributes.Collect())
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (
                    candidate:                          comb.Left.Left.Left.Left,
                    compilation:                        comb.Left.Left.Left.Right,
                    options:                            comb.Left.Left.Right,
                    disableCodegenerationAttributes:    comb.Left.Right,
                    parseOptions:                       comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false 
                && t.candidate != null
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp,
                        (CSharpCompilation)sr.compilation,
                        (CSharpParseOptions)sr.parseOptions,
                        GetCompilationContext(sr.compilation)),
                    new InterfaceProcessor(sr.candidate!),
                    sp.CancellationToken));

            var delegates = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => SyntaxHelpers.IsMock(sn),
                (ctx, ct) => ConvertToDelegate(ctx));
            context.RegisterSourceOutput(delegates
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(disableCodegenerationAttributes.Collect())
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (
                    candidate:                          comb.Left.Left.Left.Left,
                    compilation:                        comb.Left.Left.Left.Right,
                    options:                            comb.Left.Left.Right,
                    disableCodegenerationAttributes:    comb.Left.Right,
                    parseOptions:                       comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false
                && t.candidate != null
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp,
                        (CSharpCompilation)sr.compilation,
                        (CSharpParseOptions)sr.parseOptions,
                        GetCompilationContext(sr.compilation)),
                    new DelegateProcessor(sr.candidate!),
                    sp.CancellationToken));

            var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => SyntaxHelpers.IsMock(sn),
                (ctx, ct) => ConvertToAbstractClass(ctx));
            var dontOverrideTypes = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is AttributeSyntax @as && SyntaxHelpers.IsDontOverrideAttribute(@as),
                (ctx, ct) => CovertToDontOverride(ctx.SemanticModel, (AttributeSyntax)ctx.Node));
            context.RegisterSourceOutput(classes
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(disableCodegenerationAttributes.Collect())
                .Combine(context.ParseOptionsProvider)
                .Combine(dontOverrideTypes.Collect())
                .Select((comb, ct) => (
                    candidate:                          comb.Left.Left.Left.Left.Left,
                    compilation:                        comb.Left.Left.Left.Left.Right,
                    options:                            comb.Left.Left.Left.Right,
                    disableCodegenerationAttributes:    comb.Left.Left.Right,
                    parseOptions:                       comb.Left.Right,
                    dontOverrideTypes:                  comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false
                && t.candidate != null
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateCode(
                    new CodeGenerationContext(sp,
                        (CSharpCompilation)sr.compilation,
                        (CSharpParseOptions)sr.parseOptions,
                        GetCompilationContext(sr.compilation)),
                    new AbstractClassProcessor(sr.candidate!.Value.mock, sr.candidate!.Value.mockedType, sr.dontOverrideTypes),
                    sp.CancellationToken));

            var invocations = context.SyntaxProvider.CreateSyntaxProvider(
                (sn, ct) => sn is InvocationExpressionSyntax ies && SyntaxHelpers.IsArrangeInvocation(ies),
                (ctx, ct) => syntaxHelpers.ConvertToInvocation(ctx.Node, ctx.SemanticModel, ct));
            context.RegisterSourceOutput(invocations
                .Combine(context.CompilationProvider)
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(disableCodegenerationAttributes.Collect())
                .Combine(context.ParseOptionsProvider)
                .Select((comb, ct) => (
                    candidate: comb.Left.Left.Left.Left,
                    compilation: comb.Left.Left.Left.Right,
                    options: comb.Left.Left.Right,
                    disableCodegenerationAttributes: comb.Left.Right,
                    parseOptions: comb.Right))
                .Where(t
                => IsCodeGenerationDisabledByAttributes(t.disableCodegenerationAttributes)
                && IsGenerationDisabledByOptions(t.options) == false
                && t.compilation is CSharpCompilation
                && t.parseOptions is CSharpParseOptions),
                (sp, sr) => DoGenerateInvocation(
                    new CodeGenerationContext(sp,
                        (CSharpCompilation)sr.compilation,
                        (CSharpParseOptions)sr.parseOptions,
                        GetCompilationContext(sr.compilation)),
                    sr.candidate,
                    sp.CancellationToken));
        }

        static bool IsCodeGenerationDisabledByAttributes(ImmutableArray<bool> attributes)
            => attributes.Where(t => t == true).Any() == false;

        INamedTypeSymbol? ConvertToInterface(GeneratorSyntaxContext context)
        {
            var candidateGeneric = SyntaxHelpers.GetMockSymbol(context.Node);
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
            var candidateGeneric = SyntaxHelpers.GetMockSymbol(context.Node);
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
            var candidateGeneric = SyntaxHelpers.GetMockSymbol(context.Node);
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
