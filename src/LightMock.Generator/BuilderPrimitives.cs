using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
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

        public static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol)
        {
            return @this.AppendMockGetterAndSetter(contextName, symbol, sb => sb.Append(".Invoke(f => f."));
        }

        public static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol,
            string invocationType)
        {
            return @this.AppendMockGetterAndSetter(contextName, symbol,
                sb => sb.Append(".Invoke(f => ((").Append(invocationType).Append(")f)."));
        }

        static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol,
            Func<StringBuilder, StringBuilder> appendGetInvocation)
        {
            var typePart = symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.Namespace).Replace(".", "_");
            @this.Append(" {");
            if (symbol.GetMethod != null)
            {
                @this.Append(" get { ")
                    .Append(VariableNames.PropertiesContext)
                    .Append(".Invoke(f => f.")
                    .Append(symbol.Name)
                    .Append('_')
                    .Append(typePart)
                    .Append(Suffix.Getter)
                    .Append("()); return global::LightMock.Generator.Default.Get(() =>")
                    .Append(contextName);
                appendGetInvocation(@this)
                    .Append(symbol.Name)
                    .Append(")); } ");
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set { ")
                    .Append(VariableNames.PropertiesContext)
                    .Append(".Invoke(f => f.")
                    .Append(symbol.Name)
                    .Append('_')
                    .Append(typePart)
                    .Append(Suffix.Setter)
                    .Append("(value)); ")
                    .Append(contextName)
                    .Append(".InvokeSetter(f => f.")
                    .Append(symbol.Name)
                    .Append(", value); } ");
            }
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

        public static StringBuilder AppendMethodBody(this StringBuilder @this, string contextName, IMethodSymbol symbol, string invocationType)
            => AppendMethodBody(@this, contextName, symbol,
                sb => sb.Append(".Invoke(f => ((")
                .Append(invocationType)
                .Append(")f).")
                );

        public static StringBuilder AppendMethodBody(this StringBuilder @this, string contextName, IMethodSymbol symbol)
            => AppendMethodBody(@this, contextName, symbol, sb => sb.Append(".Invoke(f => f."));

        private static StringBuilder AppendMethodBody(StringBuilder @this, string contextName, IMethodSymbol symbol, Func<StringBuilder, StringBuilder> appendInvocation)
        {
            if (IsHaveRefStructParameters(symbol))
                return @this.AppendRefStructException();

            @this.Append("{");

            if (symbol.ReturnsVoid == false)
                @this.Append("return ");

            @this.Append("global::LightMock.Generator.Default.Get(() =>")
                .Append(contextName);
            appendInvocation(@this).Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                @this.Append("<")
                    .Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)))
                    .Append(">");
            }
            return @this.Append("(")
                .Append(string.Join(", ", symbol.Parameters.Select(i => i.Name)))
                .Append(")));}");
        }

        static bool IsHaveRefStructParameters(IMethodSymbol symbol)
            => (from i in symbol.Parameters
                let tod = i.Type.OriginalDefinition
                where tod.IsRefLikeType && tod.IsReadOnly
                select new { }).Any();

        static StringBuilder AppendRefStructException(this StringBuilder @this)
            => @this.Append("{ throw new global::System.InvalidProgramException(\""
                + ExceptionMessages.OnRefStructMethod + "\");}");

        public static StringBuilder AppendEventAdd(this StringBuilder @this, string contextName, IEventSymbol symbol, string methodName)
            => @this.Append("add{")
            .Append(contextName)
            .Append(".")
            .Append(methodName)
            .Append("(f => f.")
            .Append(symbol.Name)
            .Append(Suffix.Add)
            .Append("(value));}");

        public static StringBuilder AppendEventRemove(this StringBuilder @this, string contextName, IEventSymbol symbol, string methodName)
            => @this.Append("remove{")
            .Append(contextName)
            .Append(".")
            .Append(methodName)
            .Append("(f => f.")
            .Append(symbol.Name)
            .Append(Suffix.Remove)
            .Append("(value));}");

        public static StringBuilder AppendEventAddRemove(this StringBuilder @this, string contextName, IEventSymbol symbol, string methodName)
        {
            @this.Append("{");
            if (symbol.AddMethod != null)
                @this.AppendEventAdd(contextName, symbol, methodName);
            if (symbol.RemoveMethod != null)
                @this.AppendEventRemove(contextName, symbol, methodName);
            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendFileName(this StringBuilder @this, INamedTypeSymbol typeSymbol)
        {
            @this.AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name, "{", "}"), "_")
                .Append(typeSymbol.Name);
            if (typeSymbol.IsGenericType)
            {
                @this.Append('{');
                foreach (var i in typeSymbol.TypeParameters.Select(i => i.Name))
                    @this.Append(i).Append(",");
                @this.Remove(@this.Length - 1, 1).Append('}');
            }
            @this.Append(Suffix.FileName);
            return @this;
        }

        public static StringBuilder AppendContainingTypes<TResult>(
            this StringBuilder @this,
            INamedTypeSymbol typeSymbol,
            Action<StringBuilder, INamedTypeSymbol> appendTypeArguments,
            string separator = "")
        {
            if (typeSymbol.ContainingType != null)
            {
                var stack = new Stack<INamedTypeSymbol>();
                for (var ts = typeSymbol.ContainingType; ts != null; ts = ts.ContainingType)
                    stack.Push(ts);
                while (stack.Count > 0)
                {
                    var ts = stack.Pop();
                    @this.Append(ts.Name);
                    appendTypeArguments(@this, ts);
                    @this.Append(separator);
                }
            }
            return @this;
        }

        public static StringBuilder AppendTypeArguments<TResult>(
            this StringBuilder @this,
            INamedTypeSymbol typeSymbol,
            Func<ITypeSymbol, TResult> selector,
            string leftBracket = "<",
            string rightBracket = ">")
        {
            if (typeSymbol.TypeArguments.Any())
            {
                @this.Append(leftBracket)
                    .Append(string.Join(", ", typeSymbol.TypeArguments.Select(selector)))
                    .Append(rightBracket);
            }
            return @this;
        }
    }
}
