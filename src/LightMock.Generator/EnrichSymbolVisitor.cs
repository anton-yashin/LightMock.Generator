using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class EnrichSymbolVisitor : SymbolVisitor<string>
    {
        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary)
                return null;
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                .Append("{");
            if (symbol.ReturnsVoid == false)
                result.Append("return ");

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

            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                .Append(" {");
            if (symbol.GetMethod != null)
            {
                result.Append(" get { return context.Invoke(f => f.")
                    .Append(symbol.Name)
                    .Append("); } ");
            }
            if (symbol.SetMethod != null)
            {
                result.Append("set { context.InvokeSetter(f => f.")
                    .Append(symbol.Name)
                    .Append(", value); } ");
            }
            result.Append("}");
            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            return base.VisitEvent(symbol);
        }
    }
}
