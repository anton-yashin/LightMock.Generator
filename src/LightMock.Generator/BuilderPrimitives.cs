/******************************************************************************
    MIT License

    Copyright (c) 2021 Anton Yashin

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*******************************************************************************
    https://github.com/anton-yashin/
*******************************************************************************/
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    static class BuilderPrimitives
    {
        public static StringBuilder AppendProtectedInterfaceGetterAndSetter(this StringBuilder @this, string contextName, IPropertySymbol symbol)
        {
            @this.Append(" {");
            if (symbol.GetMethod != null)
            {
                @this.Append(" get { return ")
                    .Append(contextName)
                    .Append(".Invoke(f => f");
                if (symbol.IsIndexer)
                {
                    @this.Append('[')
                        .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                        .Append("]");
                }
                else
                {
                    @this.Append('.')
                    .Append(symbol.Name);
                }
                @this.Append("); } ");
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set { ");
                if (symbol.GetMethod != null)
                {
                    @this.Append(contextName)
                        .Append(".InvokeSetter(f => f");
                    if (symbol.IsIndexer)
                    {
                        @this.Append('[')
                            .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                            .Append("]");
                    }
                    else
                    {
                        @this.Append('.')
                        .Append(symbol.Name);
                    }
                    @this.Append(", value);");
                }
                @this.Append(" } ");
            }
            return @this.Append("}");
        }

        public static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol)
        {
            return @this.AppendMockGetterAndSetter(contextName, symbol, sb => sb.Append(".Invoke(f => f"));
        }

        public static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol,
            string invocationType)
        {
            return @this.AppendMockGetterAndSetter(contextName, symbol,
                sb => sb.Append(".Invoke(f => ((").Append(invocationType).Append(")f)"));
        }

        static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol,
            Func<StringBuilder, StringBuilder> appendGetInvocation)
        {
            var typePart = GetPropertyTypePart(symbol);
            @this.Append(" {");
            if (symbol.GetMethod != null)
            {
                @this.Append(" get { ")
                    .Append(VariableNames.PropertiesContext)
                    .Append(".Invoke(f => f.")
                    .AppendP2FGetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                    .Append(")); return global::LightMock.Generator.Default.Get(() =>")
                    .Append(contextName);
                appendGetInvocation(@this);
                if (symbol.IsIndexer)
                {
                    @this.Append('[')
                        .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                        .Append("])); } ");
                }
                else
                { 
                    @this.Append('.')
                        .Append(symbol.Name)
                        .Append(")); } ");
                }
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set { ")
                    .Append(VariableNames.PropertiesContext)
                    .Append(".Invoke(f => f.")
                    .AppendP2FSetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: true)
                    .Append("value)); ");
                if (symbol.GetMethod != null)
                {
                    @this.Append(contextName)
                        .Append(".InvokeSetter(f => f");
                    if (symbol.IsIndexer)
                    {
                        @this.Append('[')
                            .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                            .Append("], value); ");
                    }
                    else
                    {
                        @this.Append('.')
                            .Append(symbol.Name)
                            .Append(", value); ");
                    }
                }
                @this.Append("} ");
            }
            return @this.Append("}");
        }

        public static StringBuilder AppendAssertGetterAndSetter(
            this StringBuilder @this,
            IPropertySymbol symbol)
        {
            var typePart = GetPropertyTypePart(symbol);
            @this.Append("{");

            if (symbol.GetMethod != null)
            {
                @this.Append("get { ")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .AppendP2FGetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                    .Append("), ")
                    .Append(VariableNames.Invoked)
                    .Append("); return default(")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(");}");
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set {")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .AppendP2FSetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: true)
                    .Append("value), ")
                    .Append(VariableNames.Invoked)
                    .Append("); }");
            }

            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendAssertIsAnyGetterAndSetter(
            this StringBuilder @this,
            IPropertySymbol symbol)
        {
            var typePart = GetPropertyTypePart(symbol);
            @this.Append("{");

            if (symbol.GetMethod != null)
            {
                @this.Append("get { ")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .AppendP2FGetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersIsAnyInvocation(symbol, addCommaAtEnd: false)
                    .Append("), ")
                    .Append(VariableNames.Invoked)
                    .Append("); return default(")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(");}");
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set {")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .AppendP2FSetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersIsAnyInvocation(symbol, addCommaAtEnd: true)
                    .Append("The<")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">.IsAnyValue), ")
                    .Append(VariableNames.Invoked)
                    .Append("); }");
            }

            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendArrangeOnAnyGetterAndSetter(
            this StringBuilder @this,
            IPropertySymbol symbol,
            string propertyToFuncInterfaceName)
        {
            var typePart = GetPropertyTypePart(symbol);
            @this.Append("{");

            if (symbol.GetMethod != null)
            {
                @this.Append("get { return default(")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append("); }");
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set { request.SetResult(global::LightMock.Generator.ExpressionUtils.Get<")
                    .Append(propertyToFuncInterfaceName)
                    .Append(">(f => f.")
                    .AppendP2FSetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersIsAnyInvocation(symbol, addCommaAtEnd: true)
                    .Append("The<")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">.IsAnyValue))); }");
            }

            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendArrangeOnGetterAndSetter(
            this StringBuilder @this,
            IPropertySymbol symbol,
            string propertyToFuncInterfaceName)
        {
            var typePart = GetPropertyTypePart(symbol);
            @this.Append("{");

            if (symbol.GetMethod != null)
            {
                @this.Append("get { return default(")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append("); }");
            }
            if (symbol.SetMethod != null)
            {
                @this.Append("set { request.SetResult(global::LightMock.Generator.ExpressionUtils.Get<")
                    .Append(propertyToFuncInterfaceName)
                    .Append(">(f => f.")
                    .AppendP2FSetter(symbol, typePart)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: true)
                    .Append("value))); }");
            }

            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendPropertyDefinition(this StringBuilder @this, IPropertySymbol symbol)
        {
            var propertyTypePart = GetPropertyTypePart(symbol);
            var value = symbol.IsIndexer ? "" : ("." + symbol.Name);

            if (symbol.GetMethod != null)
            {
                @this
                    .Append("[global::LightMock.Generator.OriginalNameAttribute(")
                    .Append(symbol.GetMethod.Parameters.Length)
                    .Append(", \"")
                    .Append(value)
                    .AppendIndexerParametersFormat(symbol)
                    .Append("\")] ")
                    .Append(symbol.Type, SymbolDisplayFormats.Interface)
                    .Append(' ')
                    .AppendP2FGetter(symbol, propertyTypePart)
                    .Append("(")
                    .AppendIndexerParametersDefinition(symbol, addCommaAtEnd: false)
                    .Append(");");
            }
            if (symbol.SetMethod != null)
            {
                @this
                    .Append("[global::LightMock.Generator.OriginalNameAttribute(")
                    .Append(symbol.SetMethod.Parameters.Length)
                    .Append(", \"")
                    .Append(value)
                    .AppendIndexerParametersFormat(symbol)
                    .Append(" = {")
                    .Append(symbol.Parameters.Length)
                    .Append("}\")] ")
                    .Append("void ")
                    .AppendP2FSetter(symbol, propertyTypePart)
                    .Append("(")
                    .AppendIndexerParametersDefinition(symbol, addCommaAtEnd: true)
                    .Append(symbol.Type, SymbolDisplayFormats.Interface)
                    .Append(" prm);");
            }
            return @this;
        }

        public static StringBuilder AppendIndexerParametersDefinition(this StringBuilder @this, IPropertySymbol symbol, bool addCommaAtEnd)
        {
            if (symbol.Parameters.Length > 0)
            {
                @this.Append(symbol.Parameters[0].Type, SymbolDisplayFormats.Interface)
                    .Append(" p0");
                for (int i = 1; i < symbol.Parameters.Length; i++)
                {
                    @this.Append(", ")
                        .Append(symbol.Parameters[i].Type, SymbolDisplayFormats.Interface)
                        .Append(" p").Append(i);
                }
                if (addCommaAtEnd)
                    @this.Append(", ");
            }
            return @this;
        }

        public static StringBuilder AppendIndexerParametersFormat(this StringBuilder @this, IPropertySymbol symbol)
        {
            if (symbol.Parameters.Length > 0)
            {
                @this.Append('[').Append(symbol.Parameters[0].Type, SymbolDisplayFormats.Interface).Append(" {0}");
                for (int i = 1; i < symbol.Parameters.Length; i++)
                {
                    @this.Append(", ").Append(symbol.Parameters[i].Type, SymbolDisplayFormats.Interface)
                        .Append(" {").Append(i).Append('}');
                }
                @this.Append(']');
            }
            return @this;
        }

        public static StringBuilder AppendIndexerParametersInvocation(this StringBuilder @this, IPropertySymbol symbol, bool addCommaAtEnd)
        {
            if (symbol.Parameters.Length > 0)
            {
                @this.Append(symbol.Parameters[0].Name);
                for (int i = 1; i < symbol.Parameters.Length; i++)
                    @this.Append(", ").Append(symbol.Parameters[i].Name);
                if (addCommaAtEnd)
                    @this.Append(", ");
            }
            return @this;
        }

        public static StringBuilder AppendIndexerParametersIsAnyInvocation(this StringBuilder @this, IPropertySymbol symbol, bool addCommaAtEnd)
        {
            if (symbol.Parameters.Length > 0)
            {
                @this.Append("The<")
                    .Append(symbol.Parameters[0].Type, SymbolDisplayFormats.Interface)
                    .Append(">.IsAnyValue");
                for (int i = 1; i < symbol.Parameters.Length; i++)
                {
                    @this.Append(", The<")
                        .Append(symbol.Parameters[i].Type, SymbolDisplayFormats.Interface)
                        .Append(">.IsAnyValue");
                }
                if (addCommaAtEnd)
                    @this.Append(", ");
            }
            return @this;
        }

        public static StringBuilder AppendP2FGetter(this StringBuilder @this, IPropertySymbol symbol, string typePart)
            => @this.AppendP2FName(symbol, typePart, Suffix.Getter);

        public static StringBuilder AppendP2FSetter(this StringBuilder @this, IPropertySymbol symbol, string typePart)
            => @this.AppendP2FName(symbol, typePart, Suffix.Setter);

        static StringBuilder AppendP2FName(
            this StringBuilder @this,
            IPropertySymbol symbol,
            string typePart,
            string suffix)
        {
            return symbol.IsIndexer
                ? @this.Append(typePart)
                    .Append(Suffix.Indexer)
                    .Append(suffix)
                : @this.Append(symbol.Name)
                    .Append('_')
                    .Append(typePart)
                    .Append(suffix);
        }

        public static StringBuilder AppendP2FSetter(this StringBuilder @this, IPropertySymbol symbol)
        {
            SymbolDisplayPart Mutator(SymbolDisplayPart part)
            {
                switch (part.Kind)
                {
                    case SymbolDisplayPartKind.Punctuation when part.ToString() == ".":
                        return new SymbolDisplayPart(part.Kind, part.Symbol, "_");
                    case SymbolDisplayPartKind.InterfaceName when part.ToString().StartsWith(Prefix.ProtectedToPublicInterface):
                        return new SymbolDisplayPart(part.Kind, part.Symbol,
                            part.ToString().Replace(Prefix.ProtectedToPublicInterface, ""));
                }

                return part;
            }

            return symbol.IsIndexer
                ? @this
                    .Append(symbol.ContainingType, SymbolDisplayFormats.Namespace, Mutator)
                    .Append(Suffix.Indexer)
                    .Append(Suffix.Setter)
                : @this
                    .Append(symbol.Name)
                    .Append('_')
                    .Append(symbol.ContainingType, SymbolDisplayFormats.Namespace, Mutator)
                    .Append(Suffix.Setter);
        }

        static string GetPropertyTypePart(IPropertySymbol symbol)
            => symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.Namespace).Replace(".", "_");

        public static StringBuilder AppendMethodDeclaration(this StringBuilder @this,
            SymbolDisplayFormat format,
            IMethodSymbol symbol)
        {
            return @this.AppendMethodDeclaration(format, symbol, p => p);
        }

        public static StringBuilder AppendMethodDeclaration(this StringBuilder @this,
            SymbolDisplayFormat format,
            IMethodSymbol symbol,
            Func<SymbolDisplayPart, SymbolDisplayPart> mutator)
        {
            var allowedTypeParameters = symbol.TypeParameters.Where(
                i => i.HasReferenceTypeConstraint || i.HasValueTypeConstraint)
                .ToList();
            int i = 0;
            var parts = symbol.ToDisplayParts(format);
            foreach (var span in parts.Split(sdp => sdp.ToString() == "where"))
            {
                if (i++ == 0)
                {
                    span.Aggregate(@this, (sb, p) => sb.Append(mutator(p).ToString()));
                }
                else
                {
                    var tpn = span.First(s => s.Kind == SymbolDisplayPartKind.TypeParameterName).ToString();
                    var param = allowedTypeParameters.FirstOrDefault(p => p.Name == tpn);
                    if (param != null)
                    {
                        @this.Append("where ").Append(param.Name);
                        if (param.HasReferenceTypeConstraint)
                            @this.Append(" : class ");
                        if (param.HasValueTypeConstraint)
                            @this.Append(" : struct ");
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

        public static StringBuilder AppendDummyEventAddRemove(this StringBuilder @this, IEventSymbol symbol)
        {
            @this.Append("{");
            if (symbol.AddMethod != null)
                @this.Append("add { }");
            if (symbol.RemoveMethod != null)
                @this.Append("remove { }");
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

        public static StringBuilder Append(this StringBuilder @this, ISymbol symbol, SymbolDisplayFormat format, Func<SymbolDisplayPart, SymbolDisplayPart> mutator)
        {
            return symbol.ToDisplayParts(format)
                .Aggregate<SymbolDisplayPart, StringBuilder>(@this, (sb, p) => sb.Append(mutator(p).ToString()));
        }

        public static StringBuilder Append(
            this StringBuilder @this,
            ISymbol symbol,
            SymbolDisplayFormat format,
            Func<SymbolDisplayPart, int, SymbolDisplayPart> mutator)
        {
            return symbol.ToDisplayParts(format).Select(mutator).Aggregate(@this, (sb, p) => sb.Append(p.ToString()));
        }

        public static StringBuilder Append(this StringBuilder @this, ISymbol symbol, SymbolDisplayFormat format)
        {
            return symbol.ToDisplayParts(format)
                .Aggregate<SymbolDisplayPart, StringBuilder>(@this, (sb, p) => sb.Append(p.ToString()));
        }
    }
}
