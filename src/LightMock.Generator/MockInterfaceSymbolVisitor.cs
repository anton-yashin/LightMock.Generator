using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace LightMock.Generator
{
    sealed class MockInterfaceSymbolVisitor : InterfaceSymbolVisitor
    {
        public MockInterfaceSymbolVisitor()
        { }

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
    }
}
