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
    public class LightMockGenerator : ISourceGenerator
    {
        const string KAttributeName = nameof(GenerateMockAttribute);

        readonly Lazy<string> attribute = new Lazy<string>(() => Utils.LoadResource(KAttributeName + ".cs"));

        private static readonly SymbolDisplayFormat KNamespaceDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
            );


        public LightMockGenerator()
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation is CSharpCompilation compilation &&
                context.SyntaxReceiver is LightMockSyntaxReceiver receiver &&
                compilation.SyntaxTrees.First().Options is CSharpParseOptions options)
            {
                context.AddSource(KAttributeName, SourceText.From(attribute.Value, Encoding.UTF8));

                compilation = compilation
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(attribute.Value, Encoding.UTF8), options));

                var attributeSymbol = compilation.GetTypeByMetadataName(KAttributeName);
                if (attributeSymbol == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticsDescriptors.KNoAttributeErrorDescriptor, Location.None, KAttributeName));
                    return;
                }

                foreach (var candidateClass in receiver.CandidateClasses)
                {
                    var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                    var typeSymbol = model.GetDeclaredSymbol(candidateClass);
                    if (typeSymbol == null)
                        continue;
                    var relevantAttribute = typeSymbol.GetAttributes().FirstOrDefault(
                        a => attributeSymbol.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
                    if (relevantAttribute == null)
                        continue;

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

                    var @interface = typeSymbol.Interfaces.FirstOrDefault();
                    ClassProcessor processor;
                    if (typeSymbol.BaseType != null && typeSymbol.BaseType.ToDisplayString(KNamespaceDisplayFormat) != "System.Object")
                        processor = new AbstractClassProcessor(candidateClass, typeSymbol, typeSymbol.BaseType);
                    else
                        processor = new InterfaceProcessor(compilation, candidateClass, typeSymbol, @interface);

                    if (EmitDiagnostics(context, processor.GetErrors()))
                        continue;
                    EmitDiagnostics(context, processor.GetWarnings());
                    context.AddSource(processor.FileName, processor.DoGenerate());
                }
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
            context.RegisterForSyntaxNotifications(() => new LightMockSyntaxReceiver());
        }
    }
}
