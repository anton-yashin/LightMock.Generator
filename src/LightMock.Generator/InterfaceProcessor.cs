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
        private readonly SymbolVisitor<string> refReturnVisitor;
        private readonly SymbolVisitor<string> assertImplementationVisitor;
        private readonly SymbolVisitor<string> assertIsAnyImplementationVisitor;
        private readonly string interfaceName;
        private readonly string baseNameWithTypeArguments;
        private readonly string baseNameWithCommaArguments;
        private readonly string typeArgumentsWithBrackets;
        private readonly string typeArgumentsWithUnderlines;
        private readonly string commaArguments;
        private readonly string whereClause;
        private readonly string @namespace;
        private readonly SymbolVisitor<string> arrangeOnAnyImplementationVisitor;
        private readonly SymbolVisitor<string> arrangeOnImplementationVisitor;

        public InterfaceProcessor(
            Compilation compilation,
            INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {
            typeSymbol = typeSymbol.OriginalDefinition;

            this.symbolVisitor = new InterfaceSymbolVisitor(compilation);
            this.propertyDefinitionVisitor = new PropertyDefinitionVisitor();
            this.refReturnVisitor = new RefReturnVisitor();
            this.assertImplementationVisitor = new AssertImplementationVisitor(
                compilation, SymbolDisplayFormats.Interface, null);
            this.assertIsAnyImplementationVisitor = new AssertIsAnyImplementationVisitor(
                compilation, SymbolDisplayFormats.Interface, null);

            var typeHierarchy = typeSymbol.GetTypeHierarchy();
            var typeArguments = typeHierarchy.GetTypeArguments();
            var whereClause = typeHierarchy.GetWhereClause();

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
            typeArgumentsWithUnderlines = string.Join("_", typeArguments.Select(i => i.Name));
            commaArguments = string.Join(",", typeArguments.Select(i => " "));
            this.whereClause = whereClause;
            @namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);
            var p2fInterfaceName = Prefix.PropertyToFuncInterface + interfaceName + typeArgumentsWithBrackets;
            this.arrangeOnAnyImplementationVisitor = new ArrangeOnAnyImplementationVisitor(
                compilation, SymbolDisplayFormats.Interface, p2fInterfaceName, null);
            this.arrangeOnImplementationVisitor = new ArrangeOnImplementationVisitor(
                compilation, SymbolDisplayFormats.Interface, p2fInterfaceName, null);
        }

        public override IEnumerable<Diagnostic> GetErrors()
            => Enumerable.Empty<Diagnostic>();

        public override IEnumerable<Diagnostic> GetWarnings()
            => Enumerable.Empty<Diagnostic>();

        public override SourceText DoGenerate()
        {
            var originalNameFormat = new StringBuilder()
                .Append(typeSymbol.ContainingNamespace, SymbolDisplayFormats.Namespace)
                .Append('.')
                .Append(typeSymbol.Name);
            if (typeSymbol.IsGenericType)
            {
                originalNameFormat.Append('<');
                for (int i = 0; i < typeSymbol.TypeArguments.Length; i++)
                {
                    if (i > 0)
                        originalNameFormat.Append(", ");
                    originalNameFormat.Append('{').Append(i).Append('}');
                }
                originalNameFormat.Append('>');
            }

            var members = typeSymbol.AllInterfaces.SelectMany(i => i.GetMembers()).Concat(typeSymbol.GetMembers());
            var code = $@"// <auto-generated />
#pragma warning disable
using LightMock;

namespace {@namespace}
{{
    [global::LightMock.Generator.OriginalNameAttribute({typeSymbol.TypeArguments.Length}, ""{originalNameFormat}"")]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public interface {Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        {string.Join("\r\n        ", members.Select(i => i.Accept(propertyDefinitionVisitor)))}
    }}

    [global::LightMock.Generator.OriginalNameAttribute({typeSymbol.TypeArguments.Length}, ""{originalNameFormat}"")]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public interface {Prefix.RefReturnInterface}{interfaceName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        {string.Join("\r\n        ", members.Select(i => i.Accept(refReturnVisitor)))}
    }}

    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    sealed class {Prefix.AssertWhenImplementation}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

        public {Prefix.AssertWhenImplementation}{interfaceName}(
            IMockContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context},
            Invoked {VariableNames.Invoked})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.Invoked} = {VariableNames.Invoked};
        }}

        {string.Join("\r\n        ", members.Select(i => i.Accept(assertImplementationVisitor)))}
    }}

    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    sealed class {Prefix.AssertWhenAnyImplementation}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

        public {Prefix.AssertWhenAnyImplementation}{interfaceName}(
            IMockContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context},
            Invoked {VariableNames.Invoked})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.Invoked} = {VariableNames.Invoked};
        }}

        {string.Join("\r\n        ", members.Select(i => i.Accept(assertIsAnyImplementationVisitor)))}
    }}

    sealed class {Prefix.ArrangeWhenAnyImplementation}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly global::LightMock.Generator.ILambdaRequest {VariableNames.Request};

        public {Prefix.ArrangeWhenAnyImplementation}{interfaceName}(
            global::LightMock.Generator.ILambdaRequest {VariableNames.Request})
        {{
            this.{VariableNames.Request} = {VariableNames.Request};
        }}

        {string.Join("\r\n        ", members.Select(i => i.Accept(arrangeOnAnyImplementationVisitor)))}
    }}

    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    sealed class {Prefix.ArrangeWhenImplementation}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly global::LightMock.Generator.ILambdaRequest {VariableNames.Request};

        public {Prefix.ArrangeWhenImplementation}{interfaceName}(
            global::LightMock.Generator.ILambdaRequest {VariableNames.Request})
        {{
            this.{VariableNames.Request} = {VariableNames.Request};
        }}

        {string.Join("\r\n        ", members.Select(i => i.Accept(arrangeOnImplementationVisitor)))}
    }}

    [global::LightMock.Generator.TypeKeyAttribute(typeof(global::{@namespace}.{baseNameWithCommaArguments}))]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    sealed class {Prefix.TypeByType}{interfaceName}{typeArgumentsWithUnderlines} : global::LightMock.Generator.{nameof(TypeResolver)}
    {{
        public {Prefix.TypeByType}{interfaceName}{typeArgumentsWithUnderlines}(global::System.Type contextType)
            : base(contextType)
        {{ }}

        public override global::System.Type {nameof(TypeResolver.Key)} 
        {{
            get
            {{
                return typeof(global::{@namespace}.{baseNameWithCommaArguments});
            }}
        }}
        public override global::System.Type {nameof(TypeResolver.GetInstanceType)}()
        {{
            {GetInstanceType()};
        }}
        public override global::System.Type {nameof(TypeResolver.GetPropertiesContextType)}()
        {{
            {GetPropertiesContextType()};
        }}
        public override global::System.Type {nameof(TypeResolver.GetRefReturnContextType)}()
        {{
            {GetRefReturnContextType()};
        }}
        public override global::System.Type {nameof(TypeResolver.GetAssertWhenType)}()
        {{
            {GetAssertWhenType()};
        }}
        public override global::System.Type {nameof(TypeResolver.GetAssertWhenAnyType)}()
        {{
            {GetAssertWhenAnyType()};
        }}
        public override global::System.Type {nameof(TypeResolver.GetArrangeWhenAnyType)}()
        {{
            {GetArrangeWhenAnyType()}
        }}
        public override global::System.Type {nameof(TypeResolver.GetArrangeWhenType)}()
        {{
            {GetArrangeWhenType()}
        }}
    }}

    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    partial class {Prefix.MockClass}{interfaceName}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}
        {whereClause}
    {{
        private readonly IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context};
        private readonly IInvocationContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext};
        private readonly IInvocationContext<{Prefix.RefReturnInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.RefReturnContext};

        public {Prefix.MockClass}{interfaceName}(
            IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            IInvocationContext<{Prefix.RefReturnInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.RefReturnContext})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.PropertiesContext} = {VariableNames.PropertiesContext};
            this.{VariableNames.RefReturnContext} = {VariableNames.RefReturnContext};
        }}

        public {Prefix.MockClass}{interfaceName}(
            IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            IInvocationContext<{Prefix.RefReturnInterface}{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.RefReturnContext},
            object unused)
            : this({VariableNames.Context}, {VariableNames.PropertiesContext}, {VariableNames.RefReturnContext}) {{ }}

        {string.Join("\r\n        ", members.Select(i => i.Accept(symbolVisitor)))}
    }}
}}

namespace LightMock.Generator
{{
    using global::{@namespace};

    public static partial class MockExtensions
    {{
        [global::System.Diagnostics.DebuggerStepThrough]
        {GetTypeAccessibility()} static IMockContext<global::{@namespace}.{Prefix.RefReturnInterface}{interfaceName}{typeArgumentsWithBrackets}> RefReturn{typeArgumentsWithBrackets}(this IRefReturnContext<{baseNameWithTypeArguments}> @this)
            {whereClause}
            => (IMockContext<global::{@namespace}.{Prefix.RefReturnInterface}{interfaceName}{typeArgumentsWithBrackets}>)@this.{nameof(IRefReturnContext<object>.RefReturnContext)};
    }}
}}

#pragma warning restore
";
            return SourceText.From(code, Encoding.UTF8);
        }

        string GetInstanceType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.MockClass}{interfaceName}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.MockClass}{interfaceName});";
        }

        string GetPropertiesContextType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericMockContextType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{interfaceName}<{commaArguments}>));"
                : $"return MakeMockContextType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{interfaceName}));";
        }

        string GetRefReturnContextType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericMockContextType(typeof(global::{@namespace}.{Prefix.RefReturnInterface}{interfaceName}<{commaArguments}>));"
                : $"return MakeMockContextType(typeof(global::{@namespace}.{Prefix.RefReturnInterface}{interfaceName}));";
        }

        string GetAssertWhenType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.AssertWhenImplementation}{interfaceName}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.AssertWhenImplementation}{interfaceName});";
        }

        string GetAssertWhenAnyType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.AssertWhenAnyImplementation}{interfaceName}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.AssertWhenAnyImplementation}{interfaceName});";
        }

        string GetArrangeWhenAnyType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.ArrangeWhenAnyImplementation}{interfaceName}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.ArrangeWhenAnyImplementation}{interfaceName});";
        }

        string GetArrangeWhenType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.ArrangeWhenImplementation}{interfaceName}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.ArrangeWhenImplementation}{interfaceName});";
        }

        string GetTypeAccessibility()
        {
            switch (typeSymbol.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    return "public";
            }
            return "internal";
        }
    }
}
