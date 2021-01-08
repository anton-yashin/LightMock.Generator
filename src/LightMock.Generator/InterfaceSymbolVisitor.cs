using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class InterfaceSymbolVisitor : SymbolVisitor<string>
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

        private readonly NullableContextOptions nullableContextOptions;

        public InterfaceSymbolVisitor(NullableContextOptions nullableContextOptions)
        {
            this.nullableContextOptions = nullableContextOptions;
        }

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary)
                return null;
            var result = new StringBuilder(symbol.ToDisplayString(KSymbolDisplayFormat))
                .AppendMethodBody(VariableNames.Context, symbol);

            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            var result = new StringBuilder(symbol.ToDisplayString(KSymbolDisplayFormat))
                .AppendGetterAndSetter(VariableNames.Context, symbol);

            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            bool nullableEnabled = nullableContextOptions != NullableContextOptions.Disable;
            var localName = symbol.ContainingType.ToDisplayString(KSymbolDisplayFormat)
                .Replace(".", "")
                .Replace("<", "_")
                .Replace(">", "_") + symbol.Name;
            var result = new StringBuilder("public event ");
            result.Append(symbol.Type.ToDisplayString(KSymbolDisplayFormat))
                .Append(nullableEnabled ? "? " : " ")
                .Append(localName)
                .Append(";\r\n")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat))
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
            return symbol.ToDisplayString(KSymbolDisplayFormat);
        }
    }
}
