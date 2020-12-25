using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class AbstractClassSymbolVisitor : SymbolVisitor<string>
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



        private readonly NullableContextOptions nullableContextOptions;

        public AbstractClassSymbolVisitor(NullableContextOptions nullableContextOptions)
        {
            this.nullableContextOptions = nullableContextOptions;
        }

        bool ImplementAsInterface(ISymbol symbol)
            => (symbol.IsAbstract || symbol.IsVirtual)
                && symbol.DeclaredAccessibility == Accessibility.Protected;


        void AddInterfaceImplementation(IMethodSymbol symbol, StringBuilder result)
        {
            var @namespace = symbol.ContainingNamespace.ToDisplayString(KInterfaceDisplayFormat);
            var ctn = symbol.ContainingType.Name;
            var raw = symbol.ToDisplayString(KInterfaceDisplayFormat);
            var withInterface = raw.Replace(@namespace + "." + ctn, @namespace + "." + "IP2P_" + ctn);
            result.Append(withInterface);

            result.Append("{");

            if (symbol.ReturnsVoid == false)
                result.Append("return ");

            result.Append("protectedContext.Invoke(f => f.");
            result.Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                result.Append("<");
                result.Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)));
                result.Append(">");
            }
            result.Append("(");
            result.Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)));
            result.Append("));}");
        }

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary)
                return null;
            var result = new StringBuilder();
            bool implementAsInterface = ImplementAsInterface(symbol);

            if (implementAsInterface)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat));
            
            result.Append("{");

            if (symbol.ReturnsVoid == false)
                result.Append("return ");

            if (implementAsInterface)
                result.Append("protectedContext.Invoke(f => f.");
            else
                result.Append("context.Invoke(f => f.");
            result.Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                result.Append("<");
                result.Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)));
                result.Append(">");
            }
            result.Append("(");
            result.Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)));
            result.Append("));}");

            var s = result.ToString();

            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            bool implementAsInterface = ImplementAsInterface(symbol);

            var result = new StringBuilder();
            if (implementAsInterface)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat))
                .Append(" {");
            if (symbol.GetMethod != null)
            {
                if (implementAsInterface)
                    result.Append(" get { return protectedContext.Invoke(f => f.");
                else
                    result.Append(" get { return context.Invoke(f => f.");
                result.Append(symbol.Name)
                .Append("); } ");
            }
            if (symbol.SetMethod != null)
            {
                if (implementAsInterface)
                    result.Append("set { protectedContext.InvokeSetter(f => f.");
                else
                    result.Append("set { context.InvokeSetter(f => f.");
                result.Append(symbol.Name)
                    .Append(", value); } ");
            }
            result.Append("}");

            var s = result.ToString();

            return result.ToString();
        }

        private void AddInterfaceImplementation(IPropertySymbol symbol, StringBuilder result)
        {
            var @namespace = symbol.ContainingNamespace.ToDisplayString(KInterfaceDisplayFormat);
            var ctn = symbol.ContainingType.Name;
            var raw = symbol.ToDisplayString(KInterfaceDisplayFormat);
            var withInterface = raw.Replace(@namespace + "." + ctn, @namespace + "." + "IP2P_" + ctn);
            result.Append(withInterface);

            result.Append(" {");
            if (symbol.GetMethod != null)
            {
                result.Append(" get { return protectedContext.Invoke(f => f.")
                    .Append(symbol.Name)
                    .Append("); } ");
            }
            if (symbol.SetMethod != null)
            {
                result.Append("set { protectedContext.InvokeSetter(f => f.")
                    .Append(symbol.Name)
                    .Append(", value); } ");
            }
            result.Append("}");
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
            var s = result.ToString();
            return result.ToString();
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(KSymbolDisplayFormat);
        }
    }
}
