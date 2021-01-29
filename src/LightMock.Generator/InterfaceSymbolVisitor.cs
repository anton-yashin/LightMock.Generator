using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    class InterfaceSymbolVisitor : SymbolVisitor<string>
    {
        public InterfaceSymbolVisitor() { }

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary)
                return null;
            var result = new StringBuilder()
                .AppendMethodDeclaration(symbol.ToDisplayString(SymbolDisplayFormats.Interface), symbol)
                .AppendMethodBody(VariableNames.Context, symbol);
            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormats.Interface))
                .AppendGetterAndSetter(VariableNames.Context, symbol);

            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            var localName = symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.Interface)
                .Replace(".", "")
                .Replace("<", "_")
                .Replace(">", "_") + symbol.Name;
            var result = new StringBuilder("public event ");
            result.Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                .Append("? ")
                .Append(localName)
                .Append(";\r\n")
                .Append(symbol.ToDisplayString(SymbolDisplayFormats.Interface))
                .Append("{ add { ")
                .Append(localName)
                .Append(" += value; } remove { ")
                .Append(localName)
                .Append(" -= value; } }")
                ;
            return result.ToString();
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.Interface);
        }
    }
}
