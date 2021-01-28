using Microsoft.CodeAnalysis;
using System;
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

        public static StringBuilder AppendMockGetter(this StringBuilder @this, string contextName, ISymbol symbol)
            => @this.Append(" get { ")
            .Append(VariableNames.PropertiesContext)
            .Append(".Invoke(f => f.")
            .Append(symbol.Name)
            .Append(Suffix.Getter)
            .Append("()); return ")
            .Append(contextName)
            .Append(".Invoke(f => f.")
            .Append(symbol.Name)
            .Append("); } ");

        public static StringBuilder AppendMockSetter(this StringBuilder @this, string contextName, ISymbol symbol)
            => @this.Append("set { ")
            .Append(VariableNames.PropertiesContext)
            .Append(".Invoke(f => f.")
            .Append(symbol.Name)
            .Append(Suffix.Setter)
            .Append("(value)); ")
            .Append(contextName)
            .Append(".InvokeSetter(f => f.")
            .Append(symbol.Name)
            .Append(", value); } ");

        public static StringBuilder AppendMockGetterAndSetter(this StringBuilder @this, string contextName, IPropertySymbol symbol)
        {
            @this.Append(" {");
            if (symbol.GetMethod != null)
                @this.AppendMockGetter(contextName, symbol);
            if (symbol.SetMethod != null)
                @this.AppendMockSetter(contextName, symbol);
            return @this.Append("}");
        }

        static readonly string[] whereSeparator = new string[] { "where" };

        public static StringBuilder AppendMethodDeclaration(this StringBuilder @this, string declaration, IMethodSymbol symbol)
        {
            var allowedTypeParameters = symbol.TypeParameters.Where(
                i => i.HasReferenceTypeConstraint || i.HasValueTypeConstraint)
                .ToList();
            int i = 0;
            foreach (var part in declaration.Split(whereSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                if (i++ == 0)
                {
                    @this.Append(part);
                }
                else
                {
                    foreach (var atp in allowedTypeParameters)
                    {
                        if (part.StartsWith(" " + atp.Name + " : "))
                        {
                            @this.Append("where ")
                                .Append(atp.Name);
                            if (atp.HasReferenceTypeConstraint)
                                @this.Append(" : class ");
                            if (atp.HasValueTypeConstraint)
                                @this.Append(" : struct ");
                            break;
                        }
                    }
                }
            }

            return @this;
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
