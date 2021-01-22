using Microsoft.CodeAnalysis;
using System.Text;

namespace LightMock.Generator
{
    sealed class ProtectedMemberSymbolVisitor : SymbolVisitor<string>
    {
        public ProtectedMemberSymbolVisitor() { }

        bool IsInterfaceRequired(ISymbol symbol)
            => (symbol.IsAbstract || symbol.IsVirtual)
                && symbol.DeclaredAccessibility == Accessibility.Protected;

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || IsInterfaceRequired(symbol) == false)
                return null;

            var result = symbol.ToDisplayString(SymbolDisplayFormats.KP2PInterfaceDeclaration) + ";";

            return result;
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (IsInterfaceRequired(symbol) == false)
                return null;
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormats.KP2PInterfaceDeclaration))
                .Append("{");

            if (symbol.GetMethod != null)
                result.Append("get;");
            if (symbol.SetMethod != null)
                result.Append("set;");
            result.Append("}");

            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            return null;
        }
    }
}
