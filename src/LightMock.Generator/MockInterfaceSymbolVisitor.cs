using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace LightMock.Generator
{
    sealed class MockInterfaceSymbolVisitor : SymbolVisitor<string>
    {
        public MockInterfaceSymbolVisitor()
        { }

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
                .AppendMockGetterAndSetter(VariableNames.Context, symbol);
            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormats.Interface))
                .AppendEventAddRemove(VariableNames.PropertiesContext, symbol, methodName: "Invoke");
            return result.ToString();
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.Interface);
        }
    }
}
