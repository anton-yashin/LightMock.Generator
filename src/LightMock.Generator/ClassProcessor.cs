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

        public abstract IEnumerable<Diagnostic> GetErrors();
        public abstract IEnumerable<Diagnostic> GetWarnings();
        public abstract SourceText DoGenerate();

        public virtual void DoGeneratePart_GetInstanceType(StringBuilder here) { }
        public virtual void DoGeneratePart_GetProtectedContextType(StringBuilder here) { }
        public virtual void DoGeneratePart_GetPropertiesContextType(StringBuilder here) { }
        public virtual void DoGeneratePart_GetAssertType(StringBuilder here) { }
        public virtual void DoGeneratePart_GetDelegate(StringBuilder here) { }

        public string FileName => typeSymbol.IsGenericType
                ? typeSymbol.Name + "{" + string.Join(",", typeSymbol.TypeParameters.Select(i => i.Name)) + "}" + Suffix.FileName
                : typeSymbol.Name + Suffix.FileName;
    }
}