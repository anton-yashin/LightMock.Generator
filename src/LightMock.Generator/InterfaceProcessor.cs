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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class InterfaceProcessor : ClassProcessor
    {
        private readonly SymbolVisitor<string> symbolVisitor;
        private readonly SymbolVisitor<string> propertyDefinitionVisitor;
        private readonly SymbolVisitor<string> assertImplementationVisitor;
        private readonly string interfaceName;
        private readonly string baseNameWithTypeArguments;
        private readonly string baseNameWithCommaArguments;
        private readonly string typeArgumentsWithBrackets;
        private readonly string commaArguments;
        private readonly string whereClause;
        private readonly string @namespace;

        public InterfaceProcessor(
            INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {
            typeSymbol = typeSymbol.OriginalDefinition;

            this.symbolVisitor = new InterfaceSymbolVisitor();
            this.propertyDefinitionVisitor = new PropertyDefinitionVisitor();
            this.assertImplementationVisitor = new AssertImplementationVisitor(SymbolDisplayFormats.Interface);

            var (whereClause, typeArguments) = typeSymbol.GetWhereClauseAndTypeArguments();

            bool haveTypeArguments = typeSymbol.TypeArguments.Any();
            interfaceName = new StringBuilder()
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name, "_", "_"), "_")
                .Append(typeSymbol.Name).ToString();
            baseNameWithTypeArguments = new StringBuilder()
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name), ".")
                .Append(typeSymbol.Name)
                .Append(haveTypeArguments ? "<" + string.Join(",", typeSymbol.TypeArguments.Select(i => i.Name)) + ">" : "")
                .ToString();
            baseNameWithCommaArguments = new StringBuilder()
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => " "), ".")
                .Append(typeSymbol.Name)
                .Append(haveTypeArguments ? "<" + string.Join(",", typeSymbol.TypeArguments.Select(i => " ")) + ">" : "")
                .ToString();
            typeArgumentsWithBrackets = string.Join(",", typeArguments.Select(i => i.Name));
            if (typeArgumentsWithBrackets.Length > 0)
                typeArgumentsWithBrackets = "<" + typeArgumentsWithBrackets + ">";
            commaArguments = string.Join(",", typeArguments.Select(i => " "));
            this.whereClause = whereClause;
            @namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);
        }

        public override IEnumerable<Diagnostic> GetErrors()
            => Enumerable.Empty<Diagnostic>();

        public override IEnumerable<Diagnostic> GetWarnings()
            => Enumerable.Empty<Diagnostic>();

        public override SourceText DoGenerate()
        {
            var members = typeSymbol.AllInterfaces.SelectMany(i => i.GetMembers()).Concat(typeSymbol.GetMembers());
            var code = $@"// <auto-generated />
using LightMock;

namespace {@namespace}
{{
    public interface {Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(propertyDefinitionVisitor)))}
    }}

    sealed class {Prefix.AssertImplementation}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

        public {Prefix.AssertImplementation}{interfaceName}(
            IMockContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context},
            Invoked {VariableNames.Invoked})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.Invoked} = {VariableNames.Invoked};
        }}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(assertImplementationVisitor)))}
    }}

    partial class {Prefix.MockClass}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context};
        private readonly IInvocationContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext};

        public {Prefix.MockClass}{interfaceName}(
            IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.PropertiesContext} = {VariableNames.PropertiesContext};
        }}

        public {Prefix.MockClass}{interfaceName}(
            IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            object unused)
            : this({VariableNames.Context}, {VariableNames.PropertiesContext}) {{ }}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(symbolVisitor)))}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public override void DoGeneratePart_GetInstanceType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType 
                ? $"if (gtd == typeof(global::{@namespace}.{baseNameWithCommaArguments})) return typeof(global::{@namespace}.{Prefix.MockClass}{interfaceName}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());" 
                : $"if (contextType == typeof(global::{@namespace}.{baseNameWithCommaArguments})) return typeof(global::{@namespace}.{Prefix.MockClass}{interfaceName});";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetPropertiesContextType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $"if (gtd == typeof(global::{@namespace}.{baseNameWithCommaArguments})) return MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{interfaceName}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments()));"
                : $"if (contextType == typeof(global::{@namespace}.{baseNameWithCommaArguments})) return MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{interfaceName}));";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetAssertType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $"if (gtd == typeof(global::{@namespace}.{baseNameWithCommaArguments})) return typeof(global::{@namespace}.{Prefix.AssertImplementation}{interfaceName}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());"
                : $"if (contextType == typeof(global::{@namespace}.{baseNameWithCommaArguments})) return typeof(global::{@namespace}.{Prefix.AssertImplementation}{interfaceName});";
            here.Append(toAppend);
        }
    }
}
