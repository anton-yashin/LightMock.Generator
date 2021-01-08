using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    static class BuilderPrimitives
    {
        public static StringBuilder AppendGetter(this StringBuilder @this, string contextName, ISymbol symbol)
            => @this.Append(" get { return ")
            .Append(contextName)
            .Append(".Invoke(f => f.")
            .Append(symbol.Name)
            .Append("); } ");

        public static StringBuilder AppendSetter(this StringBuilder @this, string contextName, ISymbol symbol)
            => @this.Append("set { ")
            .Append(contextName)
            .Append(".InvokeSetter(f => f.")
            .Append(symbol.Name)
            .Append(", value); } ");

        public static StringBuilder AppendGetterAndSetter(this StringBuilder @this, string contextName, IPropertySymbol symbol)
        {
            @this.Append(" {");
            if (symbol.GetMethod != null)
                @this.AppendGetter(contextName, symbol);
            if (symbol.SetMethod != null)
                @this.AppendSetter(contextName, symbol);
            return @this.Append("}");
        }

        public static StringBuilder AppendMethodBody(this StringBuilder @this, string contextName, IMethodSymbol symbol)
        {
            @this.Append("{");

            if (symbol.ReturnsVoid == false)
                @this.Append("return ");

            @this.Append(contextName)
                .Append(".Invoke(f => f.")
                .Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                @this.Append("<")
                    .Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)))
                    .Append(">");
            }
            return @this.Append("(")
                .Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)))
                .Append("));}");
        }
    }
}
