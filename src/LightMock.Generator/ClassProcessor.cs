using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    abstract class ClassProcessor
    {
        protected readonly INamedTypeSymbol typeSymbol;

        protected ClassProcessor(
            INamedTypeSymbol typeSymbol)
        {
            this.typeSymbol = typeSymbol;
        }

        protected const string KGeneratedFileSuffix = ".spg.g.cs";
        protected static readonly SymbolDisplayFormat KNamespaceDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
            );

        public abstract IEnumerable<Diagnostic> GetErrors();
        public abstract IEnumerable<Diagnostic> GetWarnings();
        public abstract SourceText DoGenerate();

        public virtual void DoGeneratePart_CreateMockInstance(StringBuilder here) { }
        public virtual void DoGeneratePart_CreateProtectedContext(StringBuilder here) { }

        public virtual string FileName => typeSymbol.IsGenericType
                ? typeSymbol.Name + "{" + string.Join(",", typeSymbol.TypeParameters.Select(i => i.Name)) + "}" + KGeneratedFileSuffix
                : typeSymbol.Name + KGeneratedFileSuffix;
    }
}