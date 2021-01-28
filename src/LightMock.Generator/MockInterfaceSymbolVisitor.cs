using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace LightMock.Generator
{
    sealed class MockInterfaceSymbolVisitor : InterfaceSymbolVisitor
    {
        public MockInterfaceSymbolVisitor(NullableContextOptions nullableContextOptions)
            : base(nullableContextOptions)
        { }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormats.Interface))
                .AppendMockGetterAndSetter(VariableNames.Context, symbol);
            return result.ToString();
        }
    }
}
