using Microsoft.CodeAnalysis;
using System.Text;

namespace LightMock.Generator
{
    sealed class MockAbstractClassSymbolVisitor : AbstractClassSymbolVisitor
    {
        public MockAbstractClassSymbolVisitor(string interfaceNamespace, string interfaceName)
            : base(interfaceNamespace: interfaceNamespace, interfaceName: interfaceName)
        { }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (IsCanBeOverriden(symbol) == false)
                return null;

            bool isInterfaceRequired = IsInterfaceRequired(symbol);

            var result = new StringBuilder();
            if (isInterfaceRequired)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass))
                .AppendMockGetterAndSetter(isInterfaceRequired ? VariableNames.ProtectedContext : VariableNames.Context, symbol);
            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            if (IsCanBeOverriden(symbol))
            {
                var result = new StringBuilder("override ")
                    .Append(symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass))
                    .AppendEventAddRemove(VariableNames.PropertiesContext, symbol, methodName: "Invoke");
                return result.ToString();
            }
            return null;
        }
    }
}
