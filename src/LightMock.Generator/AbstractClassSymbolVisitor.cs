using Microsoft.CodeAnalysis;
using System.Text;

namespace LightMock.Generator
{
    sealed class AbstractClassSymbolVisitor : SymbolVisitor<string>
    {
        readonly string interfaceNamespace;
        readonly string interfaceName;

        public AbstractClassSymbolVisitor(string interfaceNamespace, string interfaceName)
        {
            this.interfaceNamespace = interfaceNamespace;
            this.interfaceName = interfaceName;
        }

        static bool IsInterfaceRequired(ISymbol symbol)
            => IsCanBeOverriden(symbol) && symbol.DeclaredAccessibility == Accessibility.Protected;

        static bool IsCanBeOverriden(ISymbol symbol)
            => symbol.IsAbstract || symbol.IsVirtual;

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || IsCanBeOverriden(symbol) == false)
                return null;

            var result = new StringBuilder();
            bool isInterfaceRequired = IsInterfaceRequired(symbol);

            if (isInterfaceRequired)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .AppendMethodDeclaration(symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass), symbol)
                .AppendMethodBody(isInterfaceRequired ? VariableNames.ProtectedContext : VariableNames.Context, symbol);
            return result.ToString();
        }

        void AddInterfaceImplementation(IMethodSymbol symbol, StringBuilder result)
        {
            result.AppendMethodDeclaration(CombineWithInterface(symbol), symbol)
                .AppendMethodBody(VariableNames.ProtectedContext, symbol);
        }

        private string CombineWithInterface(ISymbol symbol)
        {
            var @namespace = symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);
            var ctn = symbol.ContainingType.Name;
            return symbol
                .ToDisplayString(SymbolDisplayFormats.Interface)
                .Replace(@namespace + "." + ctn, interfaceNamespace + "." + interfaceName);
        }


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

        void AddInterfaceImplementation(IPropertySymbol symbol, StringBuilder result)
        {
            result.Append(CombineWithInterface(symbol))
                .AppendGetterAndSetter(VariableNames.ProtectedContext, symbol);
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

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass);
        }
    }
}
