using System;
using System.Collections.Generic;
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
        const string KAttributeName = nameof(GenerateMockAttribute);
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
                context.AddSource(KMock + Suffix.FileName, mock.Value);
                context.AddSource(KContextResolver + Suffix.FileName, contextResolver.Value);

                compilation = compilation
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(mock.Value, options))
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(contextResolver.Value, options));

                var attributeSymbol = compilation.GetTypeByMetadataName(KAttributeName);
                if (attributeSymbol == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticsDescriptors.KNoAttributeErrorDescriptor, Location.None, KAttributeName));
                    return;
                }

                // process symbols marked by GenerateMockAttribute

                foreach (var candidateClass in receiver.CandidateClasses)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                    var typeSymbol = model.GetDeclaredSymbol(candidateClass);
                    if (typeSymbol == null)
                        continue;
                    var relevantAttribute = typeSymbol.GetAttributes().FirstOrDefault(
                        a => attributeSymbol.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
                    if (relevantAttribute == null)
                        continue;

                    context.CancellationToken.ThrowIfCancellationRequested();
                    var isPartial = candidateClass
                        .Modifiers
                        .Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
                    if (isPartial == false)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticsDescriptors.KNoPartialKeyworkErrorDescriptor,
                            Location.Create(candidateClass.SyntaxTree, new TextSpan()),
                            typeSymbol.Name));
                        continue;
                    }

                    context.CancellationToken.ThrowIfCancellationRequested();
                    var @interface = typeSymbol.Interfaces.FirstOrDefault();
                    ClassProcessor processor;
                    if (typeSymbol.BaseType != null && typeSymbol.BaseType.ToDisplayString(SymbolDisplayFormats.Namespace) != "System.Object")
                        processor = new AbstractClassProcessor(candidateClass, typeSymbol, typeSymbol.BaseType);
                    else
                        processor = new InterfaceProcessor(compilation, candidateClass, typeSymbol, @interface);

                    if (EmitDiagnostics(context, processor.GetErrors()))
                        continue;
                    context.CancellationToken.ThrowIfCancellationRequested();
                    EmitDiagnostics(context, processor.GetWarnings());
                    context.CancellationToken.ThrowIfCancellationRequested();
                    context.AddSource(processor.FileName, processor.DoGenerate());
                }

                // process symbols under Mock<> generic

                var mockContextType = typeof(AbstractMock<>);
                var mockContextNamespaceAndName = mockContextType.Namespace + "." + mockContextType.Name.Replace("`1", "");
                var getInstanceTypeBuilder = new StringBuilder();
                var getProtectedContextTypeBuilder = new StringBuilder();
                var getPropertiesContextTypeBuilder = new StringBuilder();
                var getAssertTypeBuilder = new StringBuilder();
                var getDelegateBuilder = new StringBuilder();
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
                        && mcbt.ToDisplayString(SymbolDisplayFormats.Namespace) == mockContextNamespaceAndName
                        && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType
                        && processedTypes.Contains(mockedType) == false)
                    {
                        ClassProcessor processor;
                        var mtbt = mockedType.BaseType;
                        if (mtbt != null)
                        {
                            if (mtbt.ToDisplayString(SymbolDisplayFormats.Namespace) == multicastDelegateNameSpaceAndName)
                                processor = new MockDelegateProcessor(mockedType);
                            else
                                processor = new MockAbstractClassProcessor(candidateGeneric, mockedType);
                        }
                        else
                            processor = new MockInterfaceProcessor(mockedType);

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
                        processedTypes.Add(mockedType);
                    }
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                var impl = Utils.LoadResource(KContextResolver + Suffix.ImplFile + Suffix.CSharpFile)
                    .Replace("/*getInstanceTypeBuilder*/", getInstanceTypeBuilder.ToString())
                    .Replace("/*getProtectedContextTypeBuilder*/", getProtectedContextTypeBuilder.ToString())
                    .Replace("/*getPropertiesContextTypeBuilder*/", getPropertiesContextTypeBuilder.ToString())
                    .Replace("/*getAssertTypeBuilder*/", getAssertTypeBuilder.ToString())
                    .Replace("/*getDelegateBuilder*/", getDelegateBuilder.ToString());

                context.CancellationToken.ThrowIfCancellationRequested();
                context.AddSource(KContextResolver + Suffix.ImplFile + Suffix.FileName, SourceText.From(impl, Encoding.UTF8));
            }
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
