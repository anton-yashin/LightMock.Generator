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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class AbstractClassProcessor : ClassProcessor
    {
        private readonly string @namespace;
        private readonly SymbolVisitor<string> symbolVisitor;
        private readonly string className;
        private readonly string baseNameWithTypeArguments;
        private readonly string baseNameWithCommaArguments;
        private readonly string typeArgumentsWithBrackets;
        private readonly string whereClause;
        private readonly string typeArgumentsWithUnderlines;
        private readonly string commaArguments;
        private readonly List<string> constructors;
        private readonly List<string> constructorsCall;
        private readonly SyntaxNode containingGeneric;
        private readonly IReadOnlyList<INamedTypeSymbol> dontOverrideList;
        private readonly SymbolVisitor<string> protectedVisitor;
        private readonly SymbolVisitor<string> propertyDefinitionVisitor;
        private readonly SymbolVisitor<string> assertImplementationVisitor;
        private readonly SymbolVisitor<string> assertIsAnyImplementationVisitor;
        private readonly SymbolVisitor<string> arrangeOnAnyImplementationVisitor;
        private readonly SymbolVisitor<string> arrangeOnImplementationVisitor;

        public AbstractClassProcessor(
            SyntaxNode containingGeneric,
            INamedTypeSymbol typeSymbol,
            IReadOnlyList<INamedTypeSymbol> dontOverrideList) : base(typeSymbol)
        {
            typeSymbol = typeSymbol.OriginalDefinition;

            @namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);

            var typeHierarchy = typeSymbol.GetTypeHierarchy();
            var typeArguments = typeHierarchy.GetTypeArguments();
            var whereClause = typeHierarchy.GetWhereClause();

            bool haveTypeArguments = typeSymbol.TypeArguments.Any();

            className = new StringBuilder()
                .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name, "_", "_"), "_")
                .Append(typeSymbol.Name)
                .ToString();
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
            symbolVisitor = new AbstractClassSymbolVisitor(className);

            typeArgumentsWithBrackets = string.Join(",", typeArguments.Select(i => i.Name)); ;
            if (typeArgumentsWithBrackets.Length > 0)
                typeArgumentsWithBrackets = "<" + typeArgumentsWithBrackets + ">";
            this.whereClause = whereClause;
            typeArgumentsWithUnderlines = string.Join("_", typeArguments.Select(i => i.Name));
            commaArguments = string.Join(",", typeArguments.Select(i => " "));
            constructors = new List<string>(
                typeSymbol.Constructors.Select(
                    i => i.ToDisplayString(SymbolDisplayFormats.ConstructorDecl)
                    .Replace(typeSymbol.Name, "").Trim('(', ')')));
            constructorsCall = new List<string>(
                typeSymbol.Constructors.Select(
                    i => i.ToDisplayString(SymbolDisplayFormats.ConstructorCall)
                    .Replace(typeSymbol.Name, "").Trim('(', ')')));
            this.containingGeneric = containingGeneric;
            this.dontOverrideList = dontOverrideList;

            var p2fInterfaceName = Prefix.PropertyToFuncInterface + className + typeArgumentsWithBrackets;

            protectedVisitor = new ProtectedMemberSymbolVisitor();
            propertyDefinitionVisitor = new PropertyDefinitionVisitor();
            assertImplementationVisitor = new AssertImplementationVisitor(SymbolDisplayFormats.AbstractClass, className);
            assertIsAnyImplementationVisitor = new AssertIsAnyImplementationVisitor(SymbolDisplayFormats.AbstractClass, className);
            this.arrangeOnAnyImplementationVisitor = new ArrangeOnAnyImplementationVisitor(
                SymbolDisplayFormats.AbstractClass, p2fInterfaceName, className);
            this.arrangeOnImplementationVisitor = new ArrangeOnImplementationVisitor(
                SymbolDisplayFormats.AbstractClass, p2fInterfaceName, className);
        }

        string GenerateConstructor(string declaration, string call)
        {
            return $@"
        public {Prefix.MockClass}{className}(IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            IInvocationContext<{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext},
            {declaration})
            : base({call})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.PropertiesContext} = {VariableNames.PropertiesContext};
            this.{VariableNames.ProtectedContext} = {VariableNames.ProtectedContext};
        }}
";
        }

        string GenerateDefaultConstructor()
        {
            return $@"
        public {Prefix.MockClass}{className}(IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            IInvocationContext<{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.PropertiesContext} = {VariableNames.PropertiesContext};
            this.{VariableNames.ProtectedContext} = {VariableNames.ProtectedContext};
        }}
";
        }

        IEnumerable<string> GenerateConstructors()
        {
            for (int i = 0; i < constructors.Count; i++)
            {
                yield return constructors[i].Length == 0
                    ? GenerateDefaultConstructor()
                    : GenerateConstructor(constructors[i], constructorsCall[i]);
            }
        }

        string GenerateAssertConstructor(string prefix, string declaration, string call)
        {
            return $@"
        public {prefix}{className}(
            IMockContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.Context},
            Invoked {VariableNames.Invoked},
            {declaration})
            : base({call})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.Invoked} = {VariableNames.Invoked};
        }}
";
        }

        string GenerateAssertDefaultConstructor(string prefix)
        {
            return $@"
        public {prefix}{className}(
            IMockContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.Context},
            Invoked {VariableNames.Invoked})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.Invoked} = {VariableNames.Invoked};
        }}
";
        }

        IEnumerable<string> GenerateAssertConstructors(string prefix)
        {
            for (int i = 0; i < constructors.Count; i++)
            {
                yield return constructors[i].Length == 0
                    ? GenerateAssertDefaultConstructor(prefix)
                    : GenerateAssertConstructor(prefix, constructors[i], constructorsCall[i]);
            }
        }

        string GenerateArrangeConstructor(string prefix, string declaration, string call)
        {
            return $@"
        public {prefix}{className}(
            global::LightMock.Generator.{nameof(ILambdaRequest)} {VariableNames.Request},
            {declaration})
            : base({call})
        {{
            this.{VariableNames.Request} = {VariableNames.Request};
        }}
";
        }

        string GenerateArrangeDefaultConstructor(string prefix)
        {
            return $@"
        public {prefix}{className}(
            global::LightMock.Generator.{nameof(ILambdaRequest)} {VariableNames.Request})
        {{
            this.{VariableNames.Request} = {VariableNames.Request};
        }}
";
        }

        IEnumerable<string> GenerateArrangeConstructors(string prefix)
        {
            for (int i = 0; i < constructors.Count; i++)
            {
                yield return constructors[i].Length == 0
                    ? GenerateArrangeDefaultConstructor(prefix)
                    : GenerateArrangeConstructor(prefix, constructors[i], constructorsCall[i]);
            }
        }


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

            var members = GetAllBaseTypes(typeSymbol)
                .SelectMany(i => i.GetMembers())
                .Concat(typeSymbol.GetMembers())
                .Where(i => i.IsAbstract
                    || (i.IsVirtual
                        && dontOverrideList.Contains(i.ContainingType, SymbolEqualityComparer.Default) == false));
            var code = $@"// <auto-generated />
#pragma warning disable
using LightMock;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace {@namespace}
{{
    [global::LightMock.Generator.OriginalNameAttribute({typeSymbol.TypeArguments.Length}, ""{originalNameFormat}"")]
    public interface {Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}
    {{
        {string.Join("\r\n        ", members.Select(i => i.Accept(propertyDefinitionVisitor)))}
    }}

    [global::LightMock.Generator.OriginalNameAttribute({typeSymbol.TypeArguments.Length}, ""{originalNameFormat}"")]
    public interface {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        {string.Join("\r\n        ", members.Select(i => i.Accept(protectedVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}

    sealed class {Prefix.AssertWhenImplementation}{className}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}, {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

{string.Join("\r\n", GenerateAssertConstructors(Prefix.AssertWhenImplementation))}

        {string.Join("\r\n        ", members.Select(i => i.Accept(assertImplementationVisitor)))}
    }}

    sealed class {Prefix.AssertWhenAnyImplementation}{className}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}, {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

{string.Join("\r\n", GenerateAssertConstructors(Prefix.AssertWhenAnyImplementation))}

        {string.Join("\r\n        ", members.Select(i => i.Accept(assertIsAnyImplementationVisitor)))}
    }}

    sealed class {Prefix.ArrangeWhenAnyImplementation}{className}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}, {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly global::LightMock.Generator.{nameof(ILambdaRequest)} {VariableNames.Request};

{string.Join("\r\n", GenerateArrangeConstructors(Prefix.ArrangeWhenAnyImplementation))}

        {string.Join("\r\n        ", members.Select(i => i.Accept(arrangeOnAnyImplementationVisitor)))}
    }}

    sealed class {Prefix.ArrangeWhenImplementation}{className}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}, {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly global::LightMock.Generator.{nameof(ILambdaRequest)} {VariableNames.Request};

{string.Join("\r\n", GenerateArrangeConstructors(Prefix.ArrangeWhenImplementation))}

        {string.Join("\r\n        ", members.Select(i => i.Accept(arrangeOnImplementationVisitor)))}
    }}

    [global::LightMock.Generator.TypeKeyAttribute(typeof(global::{@namespace}.{baseNameWithCommaArguments}))]
    sealed class {Prefix.TypeByType}{className}{typeArgumentsWithUnderlines} : global::LightMock.Generator.{nameof(TypeResolver)}
    {{
        public {Prefix.TypeByType}{className}{typeArgumentsWithUnderlines}(global::System.Type contextType)
            : base(contextType)
        {{ }}

        public override global::System.Type {nameof(TypeResolver.GetInstanceType)}()
        {{
            {GetInstanceType()};
        }}
        public override global::System.Type {nameof(TypeResolver.GetProtectedContextType)}()
        {{
            {GetProtectedContextType()}
        }}
        public override global::System.Type {nameof(TypeResolver.GetPropertiesContextType)}()
        {{
            {GetPropertiesContextType()};
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

    partial class {Prefix.MockClass}{className}{typeArgumentsWithBrackets} : {baseNameWithTypeArguments}, {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IInvocationContext<{baseNameWithTypeArguments}> {VariableNames.Context};
        private readonly IInvocationContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext};
        private readonly IInvocationContext<{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext};

{string.Join("\r\n", GenerateConstructors())}

        {string.Join("\r\n        ", members.Select(i => i.Accept(symbolVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}
}}

namespace LightMock.Generator
{{
    using global::{@namespace};

    public static partial class MockExtensions
    {{
        [DebuggerStepThrough]
        {GetTypeAccessibility()} static IAdvancedMockContext<global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}> Protected{typeArgumentsWithBrackets}(this IProtectedContext<{baseNameWithTypeArguments}> @this)
            {whereClause}
            => (IAdvancedMockContext<global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}>)@this.{nameof(IProtectedContext<object>.ProtectedContext)};
    }}
}}
#pragma warning restore
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public override IEnumerable<Diagnostic> GetErrors()
        {
            if (typeSymbol.IsSealed)
            {
                yield return Diagnostic.Create(DiagnosticsDescriptors.KCantProcessSealedClass,
                    Location.Create(containingGeneric.SyntaxTree, new TextSpan()),
                    typeSymbol.Name);
            }
        }

        public override IEnumerable<Diagnostic> GetWarnings() => Enumerable.Empty<Diagnostic>();

        public override bool IsUpdateCompilationRequired => true;

        string GetInstanceType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.MockClass}{className}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.MockClass}{className});";
        }

        string GetProtectedContextType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericAdvancedMockContextType(typeof(global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}<{commaArguments}>));"
                : $"return MakeAdvancedMockContextType(typeof(global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}));";
        }

        string GetPropertiesContextType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericMockContextType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{className}<{commaArguments}>));"
                : $"return MakeMockContextType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{className}));";
        }

        string GetAssertWhenType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.AssertWhenImplementation}{className}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.AssertWhenImplementation}{className});";
        }

        string GetAssertWhenAnyType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.AssertWhenAnyImplementation}{className}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.AssertWhenAnyImplementation}{className});";
        }

        string GetArrangeWhenAnyType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.ArrangeWhenAnyImplementation}{className}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.ArrangeWhenAnyImplementation}{className});";
        }

        string GetArrangeWhenType()
        {
            return typeSymbol.IsGenericType
                ? $"return MakeGenericType(typeof(global::{@namespace}.{Prefix.ArrangeWhenImplementation}{className}<{commaArguments}>));"
                : $"return typeof(global::{@namespace}.{Prefix.ArrangeWhenImplementation}{className});";
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

        static IEnumerable<INamedTypeSymbol> GetAllBaseTypes(INamedTypeSymbol type)
        {
            for (var bt = type.BaseType; bt != null; bt = bt.BaseType)
                yield return bt;
        }
    }
}
