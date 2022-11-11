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
            return @this.AppendMockGetterAndSetter(contextName, symbol, sb => sb);
        }

        public static StringBuilder AppendMockGetterAndSetter(
            this StringBuilder @this,
            string contextName,
            IPropertySymbol symbol,
            Func<StringBuilder, StringBuilder> appendGetInvocation)
        {
            @this.Append(" {");
            if (symbol.GetMethod != null)
            {
                @this.Append(" get { ")
                    .Append(VariableNames.PropertiesContext)
                    .Append(".Invoke(f => f.")
                    .AppendP2FGetter(symbol)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: false)
                    .Append(")); return global::LightMock.Generator.Default.Get(() =>")
                    .Append(contextName)
                    .Append(".Invoke(f => (");
                appendGetInvocation(@this).Append("f)");
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
                    .AppendP2FSetter(symbol)
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
            @this.Append("{");

            if (symbol.GetMethod != null)
            {
                @this.Append("get { ")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .AppendP2FGetter(symbol)
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
                    .AppendP2FSetter(symbol)
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
            @this.Append("{");

            if (symbol.GetMethod != null)
            {
                @this.Append("get { ")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .AppendP2FGetter(symbol)
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
                    .AppendP2FSetter(symbol)
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
                    .AppendP2FSetter(symbol)
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
                    .AppendP2FSetter(symbol)
                    .Append("(")
                    .AppendIndexerParametersInvocation(symbol, addCommaAtEnd: true)
                    .Append("value))); }");
            }

            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendPropertyDefinition(this StringBuilder @this, IPropertySymbol symbol)
        {
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
                    .AppendP2FGetter(symbol)
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
                    .AppendP2FSetter(symbol)
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

        public static StringBuilder AppendP2FGetter(this StringBuilder @this, IPropertySymbol symbol)
            => @this.AppendP2FName(symbol, Suffix.Getter, ReplaceDotWithUnderline);

        public static StringBuilder AppendP2FSetter(this StringBuilder @this, IPropertySymbol symbol)
            => @this.AppendP2FName(symbol, Suffix.Setter, ReplaceDotWithUnderline);

        public static StringBuilder AppendP2FName(
            this StringBuilder @this,
            IPropertySymbol symbol,
            string suffix,
            Func<SymbolDisplayPart, SymbolDisplayPart> mutator)
        {
            return symbol.IsIndexer
                ? @this.Append(symbol.ContainingType, SymbolDisplayFormats.Namespace, mutator)
                    .Append(Suffix.Indexer)
                    .Append(suffix)
                : @this.Append(symbol.Name)
                    .Append('_')
                    .Append(symbol.ContainingType, SymbolDisplayFormats.Namespace, mutator)
                    .Append(suffix);
        }

        public static StringBuilder AppendE2FAdd(this StringBuilder @this, IEventSymbol symbol)
            => @this.AppendE2FName(symbol, Suffix.Add, ReplaceDotWithUnderline);

        public static StringBuilder AppendE2FRemove(this StringBuilder @this, IEventSymbol symbol)
            => @this.AppendE2FName(symbol, Suffix.Remove, ReplaceDotWithUnderline);

        public static StringBuilder AppendE2FName(
            this StringBuilder @this,
            IEventSymbol symbol,
            string suffix,
            Func<SymbolDisplayPart, SymbolDisplayPart> mutator)
        {
            return @this.Append(symbol.Name)
                    .Append('_')
                    .Append(symbol.ContainingType, SymbolDisplayFormats.Namespace, mutator)
                    .Append(suffix);
        }

        static SymbolDisplayPart ReplaceDotWithUnderline(SymbolDisplayPart p)
            => p.Kind == SymbolDisplayPartKind.Punctuation && p.ToString() == "."
            ? new SymbolDisplayPart(p.Kind, p.Symbol, "_")
            : p;

        public static StringBuilder AppendMethodDeclaration(this StringBuilder @this,
            Compilation compilation,
            SymbolDisplayFormat format,
            IMethodSymbol symbol)
        {
            return @this.AppendMethodDeclaration(compilation, format, symbol, p => p);
        }

        public static StringBuilder AppendMethodDeclaration(this StringBuilder @this,
            Compilation compilation,
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
                    span.FilterInternalKeywordFromNonFriendDeclarations(symbol, compilation)
                        .Aggregate(@this, (sb, p) => sb.Append(mutator(p).ToString()));
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

        public static StringBuilder AppendMethodBody(this StringBuilder @this, string contextName, IMethodSymbol symbol)
            => AppendMethodBody(@this, contextName, symbol, sb => sb);

        public static StringBuilder AppendMethodBody(
            this StringBuilder @this,
            string contextName,
            IMethodSymbol symbol,
            Func<StringBuilder, StringBuilder> appendTypeCast)
        {
            const string REF_VARIABLE_PREFIX = "__ref_variable_for_";
            const string RESULT_VARIABLE = "__result_variable";
            const string REF_VALUES_VARIABLE = "__ref_values";
            if (IsHaveRefStructParameters(symbol))
                return @this.AppendRefStructException();

            @this.Append("{");

            var refParams = symbol.Parameters.Where(
                p => p.RefKind == RefKind.Out || p.RefKind == RefKind.Ref || p.RefKind == RefKind.In);
            bool haveRefParams = false;

            foreach (var parameter in refParams)
            {
                haveRefParams = true;
                @this.Append(parameter.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(" " + REF_VARIABLE_PREFIX)
                    .Append(parameter.Name)
                    .Append(" = ")
                    .Append(parameter.RefKind == RefKind.Out ? "default" : parameter.EscapedName())
                    .Append(";");
            }

            if (haveRefParams)
            {
                const string DICTIONARY_TYPE = "global::System.Collections.Generic.Dictionary<string, object>";
                @this.Append(DICTIONARY_TYPE + " " + REF_VALUES_VARIABLE + " = new " + DICTIONARY_TYPE + "();");
            }

            if (symbol.ReturnsVoid == false)
            {
                @this.Append(symbol.ReturnType, SymbolDisplayFormats.WithTypeParams)
                    .Append(" " + RESULT_VARIABLE + " = ");
            }

            @this.Append("global::LightMock.Generator.Default.Get(() =>")
                .Append(contextName)
                .Append(".Invoke(f => (");
            appendTypeCast(@this).Append("f).").Append(symbol.Name);
            if (symbol.IsGenericMethod)
            {
                @this.Append("<")
                    .Append(string.Join(",", symbol.TypeParameters.Select(i => i.Name)))
                    .Append(">");
            }

            @this.Append("(")
                .Append(string.Join(", ", symbol.Parameters.Select(GetParam)))
                .Append(")");

            if (haveRefParams)
                @this.Append(", " + REF_VALUES_VARIABLE);
            @this.Append("));");

            foreach (var parameter in refParams)
            {
                if (parameter.RefKind == RefKind.In)
                    continue;
                @this.Append(parameter.EscapedName())
                    .Append(" = global::LightMock.ArgumentHelper.Unpack<")
                    .Append(parameter.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">(" + REF_VALUES_VARIABLE + ", nameof(")
                    .Append(parameter.Name)
                    .Append(")");
                if (parameter.RefKind == RefKind.Ref)
                    @this.Append(", ").Append(parameter.EscapedName());
                @this.Append(");");
            }

            if (symbol.ReturnsVoid)
                @this.Append("}");
            else
                @this.Append(" return " + RESULT_VARIABLE + "; }");
            return @this;

            static string GetParam(IParameterSymbol ps)
            {
                switch (ps.RefKind)
                {
                    case RefKind.Ref:
                        return "ref " + REF_VARIABLE_PREFIX + ps.Name;
                    case RefKind.Out:
                        return "out " + REF_VARIABLE_PREFIX + ps.Name;
                    case RefKind.In:
                        return "in " + REF_VARIABLE_PREFIX + ps.Name;
                }
                return ps.EscapedName();
            }
        }

        static bool IsHaveRefStructParameters(IMethodSymbol symbol)
            => (from i in symbol.Parameters
                let tod = i.Type.OriginalDefinition
                where tod.IsRefLikeType && tod.IsReadOnly
                select new { }).Any();

        static StringBuilder AppendRefStructException(this StringBuilder @this)
            => @this.Append("{ throw new global::System.InvalidProgramException(\""
                + ExceptionMessages.OnRefStructMethod + "\");}");

        public static StringBuilder AppendEventDefinition(this StringBuilder @this, IEventSymbol symbol)
        {
            if (symbol.AddMethod != null)
            {
                @this
                    .Append("[global::LightMock.Generator.OriginalNameAttribute(1, \".")
                    .Append(symbol.Name)
                    .Append(" += {0}\")] ")
                    .Append("void ")
                    .AppendE2FAdd(symbol)
                    .Append("(")
                    .Append(symbol.Type, SymbolDisplayFormats.Interface)
                    .Append(" prm);");
            }
            if (symbol.RemoveMethod != null)
            {
                @this
                    .Append("[global::LightMock.Generator.OriginalNameAttribute(1, \".")
                    .Append(symbol.Name)
                    .Append(" -= {0}\")] ")
                    .Append("void ")
                    .AppendE2FRemove(symbol)
                    .Append("(")
                    .Append(symbol.Type, SymbolDisplayFormats.Interface)
                    .Append(" prm);");
            }
            return @this;
        }

        public static StringBuilder AppendEventAddRemove(this StringBuilder @this, string contextName, IEventSymbol symbol, string methodName)
        {
            @this.Append("{");
            if (symbol.AddMethod != null)
            {
                @this.Append("add{")
                    .Append(contextName)
                    .Append(".")
                    .Append(methodName)
                    .Append("(f => f.")
                    .AppendE2FAdd(symbol)
                    .Append("(value));}");
            }
            if (symbol.RemoveMethod != null)
            {
                @this.Append("remove{")
                    .Append(contextName)
                    .Append(".")
                    .Append(methodName)
                    .Append("(f => f.")
                    .AppendE2FRemove(symbol)
                    .Append("(value));}");
            }
            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendAssertIsAnyEventAddRemove(this StringBuilder @this, string contextName, IEventSymbol symbol)
        {
            @this.Append("{");
            if (symbol.AddMethod != null)
            {
                @this.Append(" add { ")
                    .Append(contextName)
                    .Append(".Assert(f => f.")
                    .AppendE2FAdd(symbol)
                    .Append("(The<")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">.IsAnyValue)); } ");
            }
            if (symbol.RemoveMethod != null)
            {
                @this.Append(" remove { ")
                    .Append(contextName)
                    .Append(".Assert(f => f.")
                    .AppendE2FRemove(symbol)
                    .Append("(The<")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">.IsAnyValue)); } ");
            }
            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendArrangeOnAnyAddAndRemove(
            this StringBuilder @this,
            IEventSymbol symbol,
            string propertyToFuncInterfaceName)
        {
            @this.Append("{");

            if (symbol.AddMethod != null)
            {
                @this.Append("add { request.SetResult(global::LightMock.Generator.ExpressionUtils.Get<")
                    .Append(propertyToFuncInterfaceName)
                    .Append(">(f => f.")
                    .AppendE2FAdd(symbol)
                    .Append("(The<")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">.IsAnyValue))); }");
            }
            if (symbol.RemoveMethod != null)
            {
                @this.Append("remove { request.SetResult(global::LightMock.Generator.ExpressionUtils.Get<")
                    .Append(propertyToFuncInterfaceName)
                    .Append(">(f => f.")
                    .AppendE2FRemove(symbol)
                    .Append("(The<")
                    .Append(symbol.Type, SymbolDisplayFormats.WithTypeParams)
                    .Append(">.IsAnyValue))); }");
            }

            @this.Append("}");
            return @this;
        }

        public static StringBuilder AppendArrangeOnAddAndRemove(
            this StringBuilder @this,
            IEventSymbol symbol,
            string propertyToFuncInterfaceName)
        {
            @this.Append("{");

            if (symbol.AddMethod != null)
            {
                @this.Append("add { request.SetResult(global::LightMock.Generator.ExpressionUtils.Get<")
                    .Append(propertyToFuncInterfaceName)
                    .Append(">(f => f.")
                    .AppendE2FAdd(symbol)
                    .Append("(value))); }");
            }
            if (symbol.RemoveMethod != null)
            {
                @this.Append("remove { request.SetResult(global::LightMock.Generator.ExpressionUtils.Get<")
                    .Append(propertyToFuncInterfaceName)
                    .Append(">(f => f.")
                    .AppendE2FRemove(symbol)
                    .Append("(value))); }");
            }

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

        public static StringBuilder Append(
            this StringBuilder @this,
            ISymbol symbol,
            SymbolDisplayFormat format,
            Func<SymbolDisplayPart, SymbolDisplayPart> mutator)
        {
            return @this.AppendParts(symbol.ToDisplayParts(format), mutator);
        }

        public static StringBuilder Append(
            this StringBuilder @this,
            ISymbol symbol,
            SymbolDisplayFormat format,
            Func<SymbolDisplayPart, int, SymbolDisplayPart> mutator)
        {
            return @this.AppendParts(symbol.ToDisplayParts(format), mutator);
        }

        public static StringBuilder Append(this StringBuilder @this, ISymbol symbol, SymbolDisplayFormat format) 
            => @this.AppendParts(symbol.ToDisplayParts(format));

        public static StringBuilder AppendParts<T>(this StringBuilder @this, T parts)
            where T : IEnumerable<SymbolDisplayPart>
        {
            return parts.Aggregate(@this, (sb, item) => sb.Append(item.ToString()));
        }

        public static StringBuilder AppendParts<T>(this StringBuilder @this, T parts, Func<SymbolDisplayPart, SymbolDisplayPart> mutator)
            where T : IEnumerable<SymbolDisplayPart>
        {
            return parts.Aggregate(@this, (sb, item) => sb.Append(mutator(item).ToString()));
        }

        public static StringBuilder AppendParts<T>(this StringBuilder @this, T parts, Func<SymbolDisplayPart, int, SymbolDisplayPart> mutator)
            where T : IEnumerable<SymbolDisplayPart>
        {
            int i = 0;
            foreach (var item in parts)
                @this.Append(mutator(item, i++));
            return @this;
        }
    }
}
