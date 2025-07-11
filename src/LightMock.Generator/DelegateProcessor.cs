﻿/******************************************************************************
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
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LightMock.Generator
{
    sealed class DelegateProcessor : ClassProcessor
    {
        private readonly static Lazy<Regex> allowRefStructRex = new Lazy<Regex>(()
            => new Regex(@"where \w+? : allows ref struct", RegexOptions.None, TimeSpan.FromDays(1)));
        private readonly string className;
        private readonly string fullNameWithTypeArguments;
        private readonly string fullNameWithCommaArguments;
        private readonly string @namespace;
        private readonly string typeArgumentsWithBrackets;
        private readonly string typeArgumentsWithUnderlines;
        private readonly string whereClause;
        private readonly string commaArguments;
        private readonly string returnType;
        private readonly string @return;

        public DelegateProcessor(INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {
            typeSymbol = typeSymbol.OriginalDefinition;
            className = new StringBuilder(Prefix.MockClass)
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name, "_", "_"), "_")
                .Append(typeSymbol.Name)
                .Append(typeSymbol.TypeArguments.Any() ? "_" + string.Join("_", typeSymbol.TypeArguments.Select(i => i.Name)) + "_" : "")
                .ToString();
            fullNameWithTypeArguments = new StringBuilder()
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name), ".")
                .Append(typeSymbol.Name)
                .Append(typeSymbol.TypeArguments.Any() ? "<" + string.Join(",", typeSymbol.TypeArguments.Select(i => i.Name)) + ">" : "")
                .ToString();
            fullNameWithCommaArguments = new StringBuilder()
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => " "), ".")
                .Append(typeSymbol.Name)
                .Append(typeSymbol.TypeArguments.Any() ? "<" + string.Join(",", typeSymbol.TypeArguments.Select(i => " ")) + ">" : "")
                .ToString();
            @namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);

            var typeHierarchy = typeSymbol.GetTypeHierarchy();
            var typeArguments = typeHierarchy.GetTypeArguments();
            var whereClause = typeHierarchy.GetWhereClause();
            whereClause = whereClause.Replace(", allows ref struct", "");
            whereClause = allowRefStructRex.Value.Replace(whereClause, "");

            typeArgumentsWithBrackets = string.Join(",", typeArguments.Select(i => i.Name)); ;
            if (typeArgumentsWithBrackets.Length > 0)
                typeArgumentsWithBrackets = "<" + typeArgumentsWithBrackets + ">";
            typeArgumentsWithUnderlines = string.Join("_", typeArguments.Select(i => i.Name));
            this.whereClause = whereClause;
            this.commaArguments = string.Join(",", typeArguments.Select(i => " "));
            var rt = typeSymbol.DelegateInvokeMethod?.ReturnType;
            const string @void = "void";
            this.returnType = rt != null ? rt.ToDisplayString(SymbolDisplayFormats.Namespace) : @void;
            if (this.returnType == "System.Void")
                this.returnType = @void;
            this.@return = this.returnType == @void ? "" : "return ";
        }

        public override SourceText DoGenerate()
        {

            var code = $@"// <auto-generated />
#pragma warning disable
using LightMock;
using LightMock.Generator;

namespace {@namespace}
{{
    [global::LightMock.Generator.TypeKeyAttribute(typeof(global::{@namespace}.{fullNameWithCommaArguments}))]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    sealed class {Prefix.TypeByType}{className}{typeArgumentsWithUnderlines} : global::LightMock.Generator.{nameof(TypeResolver)}
    {{
        public {Prefix.TypeByType}{className}{typeArgumentsWithUnderlines}(global::System.Type contextType)
            : base(contextType)
        {{ }}

        public override global::System.Type {nameof(TypeResolver.Key)} 
        {{
            get
            {{
                return typeof(global::{@namespace}.{fullNameWithCommaArguments});
            }}
        }}

        public override global::System.Type {nameof(TypeResolver.GetInstanceType)}()
        {{
            return ContextType;
        }}

        public override object {nameof(TypeResolver.GetDelegate)}(object mockContext)
        {{
            {GetDelegate()}
        }}
    }}

    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    sealed class {className}{typeArgumentsWithBrackets} : {nameof(IDelegateProvider)}
        {whereClause}
    {{
        private readonly IInvocationContext<{fullNameWithTypeArguments}> {VariableNames.Context};

        public {className}(IInvocationContext<{fullNameWithTypeArguments}> {VariableNames.Context})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
        }}

        public {returnType} Invoke({string.Join(", ", GetParametersWithTypeNames())})
        {{
            {@return}{VariableNames.Context}.Invoke(f => f.Invoke({string.Join(", ", GetParameters())}));
        }}

        public global::System.Delegate {nameof(IDelegateProvider.GetDelegate)}()
        {{
            {fullNameWithTypeArguments} result = Invoke;
            return result;
        }}
    }}
}}
#pragma warning restore
";
            return SourceText.From(code, Encoding.UTF8);
        }

        IEnumerable<string> GetParametersWithTypeNames()
        {
            var dim = typeSymbol.OriginalDefinition.DelegateInvokeMethod;
            if (dim != null)
            {
                foreach (var i in dim.Parameters)
                {
                    yield return i.Type.ToDisplayString(SymbolDisplayFormats.Namespace) + " " + i.Name;
                }
            }
        }

        IEnumerable<string> GetParameters()
        {
            var dim = typeSymbol.OriginalDefinition.DelegateInvokeMethod;
            if (dim != null)
            {
                foreach (var i in dim.Parameters)
                {
                    yield return i.Name;
                }
            }
        }

        public override IEnumerable<Diagnostic> GetErrors()
            => Enumerable.Empty<Diagnostic>();

        public override IEnumerable<Diagnostic> GetWarnings()
            => Enumerable.Empty<Diagnostic>();

        string GetDelegate()
        {
            return typeSymbol.IsGenericType
                ? $"return CreateGenericDelegate(typeof({@namespace}.{className}<{commaArguments}>), mockContext, \"{fullNameWithTypeArguments}\");"
                : $"return new global::{@namespace}.{className}((IInvocationContext<global::{@namespace}.{fullNameWithCommaArguments}>)mockContext).GetDelegate();";
        }
    }
}
