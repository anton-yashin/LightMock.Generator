using Microsoft.CodeAnalysis;
using System.Text;

namespace LightMock.Generator
{
    sealed class MockAbstractClassSymbolVisitor : SymbolVisitor<string>
    {
        static readonly SymbolDisplayFormat KSymbolDisplayFormat = 
            new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                memberOptions:
                    SymbolDisplayMemberOptions.IncludeParameters |
                    SymbolDisplayMemberOptions.IncludeType |
                    SymbolDisplayMemberOptions.IncludeRef |
                    SymbolDisplayMemberOptions.IncludeAccessibility,
                kindOptions:
                    SymbolDisplayKindOptions.IncludeMemberKeyword,
                parameterOptions:
                    SymbolDisplayParameterOptions.IncludeName |
                    SymbolDisplayParameterOptions.IncludeType |
                    SymbolDisplayParameterOptions.IncludeParamsRefOut |
                    SymbolDisplayParameterOptions.IncludeDefaultValue,
                localOptions: SymbolDisplayLocalOptions.IncludeType,
                miscellaneousOptions:
                    SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                    SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                    SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        static readonly SymbolDisplayFormat KInterfaceDisplayFormat =
            new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                memberOptions:
                    SymbolDisplayMemberOptions.IncludeParameters |
                    SymbolDisplayMemberOptions.IncludeType |
                    SymbolDisplayMemberOptions.IncludeRef |
                    SymbolDisplayMemberOptions.IncludeContainingType,
                kindOptions:
                    SymbolDisplayKindOptions.IncludeMemberKeyword,
                parameterOptions:
                    SymbolDisplayParameterOptions.IncludeName |
                    SymbolDisplayParameterOptions.IncludeType |
                    SymbolDisplayParameterOptions.IncludeParamsRefOut |
                    SymbolDisplayParameterOptions.IncludeDefaultValue,
                localOptions: SymbolDisplayLocalOptions.IncludeType,
                miscellaneousOptions:
                    SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                    SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                    SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        public MockAbstractClassSymbolVisitor() { }

        static bool IsInterfaceRequired(ISymbol symbol)
            => IsCanBeOverriden(symbol) && symbol.DeclaredAccessibility == Accessibility.Protected;

        static bool IsCanBeOverriden(ISymbol symbol)
            => symbol.IsAbstract || symbol.IsVirtual;

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || IsCanBeOverriden(symbol) == false)
                return null;

            symbol = symbol.OriginalDefinition;
            var result = new StringBuilder();
            bool isInterfaceRequired = IsInterfaceRequired(symbol);

            if (isInterfaceRequired)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat))
                .AppendMethodBody(isInterfaceRequired ? VariableNames.ProtectedContext : VariableNames.Context, symbol);
            return result.ToString();
        }

        void AddInterfaceImplementation(IMethodSymbol symbol, StringBuilder result)
        {
            result.Append(CombineWithInterface(symbol))
                .AppendMethodBody(VariableNames.ProtectedContext, symbol);
        }

        private string CombineWithInterface(ISymbol symbol)
        {
            var @namespace = symbol.ContainingNamespace.ToDisplayString(KInterfaceDisplayFormat);
            var ctn = symbol.ContainingType.Name;
            var result = symbol
                .ToDisplayString(KInterfaceDisplayFormat)
                .Replace(@namespace + "." + ctn, Prefix.ProtectedToPublicInterface + ctn);
            return result;
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (IsCanBeOverriden(symbol) == false)
                return null;

            symbol = symbol.OriginalDefinition;
            bool isInterfaceRequired = IsInterfaceRequired(symbol);

            var result = new StringBuilder();
            if (isInterfaceRequired)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat))
                .AppendGetterAndSetter(isInterfaceRequired ? VariableNames.ProtectedContext : VariableNames.Context, symbol);

            return result.ToString();
        }

        private void AddInterfaceImplementation(IPropertySymbol symbol, StringBuilder result)
        {
            result.Append(CombineWithInterface(symbol))
                .AppendGetterAndSetter(VariableNames.ProtectedContext, symbol);
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            if (symbol.IsAbstract)
            {
                symbol = symbol.OriginalDefinition;
                var sdf = symbol.ToDisplayString(KSymbolDisplayFormat);
                var result = new StringBuilder("override ")
                    .Append(sdf)
                    .Append(";");
                return result.ToString();
            }
            return null;
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.OriginalDefinition.ToDisplayString(KSymbolDisplayFormat);
        }
    }
}
