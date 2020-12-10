using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticProxy
{
    sealed class EnrichSymbolVisitor : SymbolVisitor<string>
    {
        public override string? VisitMethod(IMethodSymbol symbol)
        {
            var result = new StringBuilder(symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                .Append("{");
            if (symbol.ReturnsVoid == false)
                result.Append("return ");

            result.Append("context.Invoke(f => f.");
            result.Append(symbol.Name);
            result.Append("(");
            result.Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)));
            result.Append("));}");

            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            return base.VisitProperty(symbol);
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            return base.VisitEvent(symbol);
        }
    }
}
