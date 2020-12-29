using Microsoft.CodeAnalysis;
using System;
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
        private readonly string interfaceNamespace;

        public AbstractClassSymbolVisitor(NullableContextOptions nullableContextOptions, string interfaceNamespace)
        {
            this.nullableContextOptions = nullableContextOptions;
            this.interfaceNamespace = interfaceNamespace;
        }

        bool ImplementAsInterface(ISymbol symbol)
            => (symbol.IsAbstract || symbol.IsVirtual)
                && symbol.DeclaredAccessibility == Accessibility.Protected;

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary
                || (symbol.IsAbstract == false && symbol.IsVirtual == false))
                return null;
            var result = new StringBuilder();
            bool implementAsInterface = ImplementAsInterface(symbol);

            if (implementAsInterface)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat))
                .Append("{");

            if (symbol.ReturnsVoid == false)
                result.Append("return ");

            result.Append(implementAsInterface
                ? "protectedContext.Invoke(f => f."
                : "context.Invoke(f => f.")
                .Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                result.Append("<")
                    .Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)))
                    .Append(">");
            }
            result.Append("(")
                .Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)))
                .Append("));}");

            return result.ToString();
        }

        void AddInterfaceImplementation(IMethodSymbol symbol, StringBuilder result)
        {
            result.Append(CombineWithInterface(symbol))
                .Append("{");

            if (symbol.ReturnsVoid == false)
                result.Append("return ");

            result.Append("protectedContext.Invoke(f => f.")
                .Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                result.Append("<")
                    .Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)))
                    .Append(">");
            }
            result.Append("(")
                .Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)))
                .Append("));}");
        }

        private string CombineWithInterface(ISymbol symbol)
        {
            var @namespace = symbol.ContainingNamespace.ToDisplayString(KInterfaceDisplayFormat);
            var ctn = symbol.ContainingType.Name;
            return symbol
                .ToDisplayString(KInterfaceDisplayFormat)
                .Replace(@namespace + "." + ctn, interfaceNamespace + "." + "IP2P_" + ctn);
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (symbol.IsAbstract == false && symbol.IsVirtual == false)
                return null;

            bool implementAsInterface = ImplementAsInterface(symbol);

            var result = new StringBuilder();
            if (implementAsInterface)
                AddInterfaceImplementation(symbol, result);

            result.Append("override ")
                .Append(symbol.ToDisplayString(KSymbolDisplayFormat))
                .Append(" {");
            if (symbol.GetMethod != null)
            {
                result.Append(implementAsInterface 
                    ? " get { return protectedContext.Invoke(f => f." 
                    : " get { return context.Invoke(f => f.")
                    .Append(symbol.Name)
                    .Append("); } ");
            }
            if (symbol.SetMethod != null)
            {
                result.Append(implementAsInterface 
                    ? "set { protectedContext.InvokeSetter(f => f." 
                    : "set { context.InvokeSetter(f => f.")
                    .Append(symbol.Name)
                    .Append(", value); } ");
            }
            result.Append("}");

            return result.ToString();
        }

        private void AddInterfaceImplementation(IPropertySymbol symbol, StringBuilder result)
        {
            result.Append(CombineWithInterface(symbol))
                .Append(" {");
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
            if (symbol.IsAbstract)
            {
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
            return symbol.ToDisplayString(KSymbolDisplayFormat);
        }
    }
}
