﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class AbstractClassProcessor : ClassProcessor
    {
        private readonly SymbolVisitor<string> protectedVisitor;
        private readonly SymbolVisitor<string> propertyDefinitionVisitor;
        private readonly SymbolVisitor<string> assertImplementationVisitor;
        private readonly string @namespace;
        private readonly SymbolVisitor<string> symbolVisitor;
        private readonly string className;
        private readonly string baseName;
        private readonly string typeArgumentsWithBrackets;
        private readonly string whereClause;
        private readonly string commaArguments;
        private readonly List<string> constructors;
        private readonly List<string> constructorsCall;
        private readonly SyntaxNode containingGeneric;

        public AbstractClassProcessor(
            SyntaxNode containingGeneric,
            INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {

            this.protectedVisitor = new ProtectedMemberSymbolVisitor();
            this.propertyDefinitionVisitor = new PropertyDefinitionVisitor();
            this.assertImplementationVisitor = new AssertImplementationVisitor(SymbolDisplayFormats.AbstractClass);
            this.@namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);
            this.className = new StringBuilder().AppendContainingTypes(typeSymbol, "_").Append(typeSymbol.Name).ToString();
            this.baseName = new StringBuilder().AppendContainingTypes(typeSymbol, ".").Append(typeSymbol.Name).ToString();
            this.symbolVisitor = new AbstractClassSymbolVisitor(@namespace, Prefix.ProtectedToPublicInterface + className);

            var to = typeSymbol.OriginalDefinition;
            var withTypeParams = to.ToDisplayString(SymbolDisplayFormats.WithTypeParams);
            var withWhereClause = to.ToDisplayString(SymbolDisplayFormats.WithWhereClause);
            var typeArguments = withTypeParams.Replace(to.ToDisplayString(SymbolDisplayFormats.Namespace), "");

            this.typeArgumentsWithBrackets = typeArguments.Length > 0 ? typeArguments : "";
            this.whereClause = withWhereClause.Replace(withTypeParams, "");
            this.commaArguments = string.Join(",",
                typeSymbol.OriginalDefinition.TypeArguments.Select(i => " "));
            this.constructors = new List<string>(
                to.Constructors.Select(
                    i => i.ToDisplayString(SymbolDisplayFormats.ConstructorDecl)
                    .Replace(typeSymbol.Name, "").Trim('(', ')')));
            this.constructorsCall = new List<string>(
                to.Constructors.Select(
                    i => i.ToDisplayString(SymbolDisplayFormats.ConstructorCall)
                    .Replace(typeSymbol.Name, "").Trim('(', ')')));
            this.containingGeneric = containingGeneric;
        }

        string GenerateConstructor(string declaration, string call)
        {
            return $@"
        public {Prefix.MockClass}{className}(IInvocationContext<{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context},
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
        public {Prefix.MockClass}{className}(IInvocationContext<{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context},
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

        string GenerateAssertConstructor(string declaration, string call)
        {
            return $@"
        public {Prefix.AssertImplementation}{className}(
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

        string GenerateAssertDefaultConstructor()
        {
            return $@"
        public {Prefix.AssertImplementation}{className}(
            IMockContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.Context},
            Invoked {VariableNames.Invoked})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.Invoked} = {VariableNames.Invoked};
        }}
";
        }

        IEnumerable<string> GenerateAssertConstructors()
        {
            for (int i = 0; i < constructors.Count; i++)
            {
                yield return constructors[i].Length == 0
                    ? GenerateAssertDefaultConstructor()
                    : GenerateAssertConstructor(constructors[i], constructorsCall[i]);
            }
        }


        public override SourceText DoGenerate()
        {
            var members = GetAllBaseTypes(typeSymbol).SelectMany(i => i.GetMembers()).Concat(typeSymbol.GetMembers());
            var code = $@"// <auto-generated />
using LightMock;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace {@namespace}
{{
    public interface {Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}
    {{
        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(propertyDefinitionVisitor)))}
    }}

    sealed class {Prefix.AssertImplementation}{className}{typeArgumentsWithBrackets} : {baseName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

{string.Join("\r\n", GenerateAssertConstructors())}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(assertImplementationVisitor)))}
    }}

    public interface {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(protectedVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}


    partial class {Prefix.MockClass}{className}{typeArgumentsWithBrackets} : {baseName}{typeArgumentsWithBrackets}, {Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IInvocationContext<{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly IInvocationContext<{Prefix.PropertyToFuncInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext};
        private readonly IInvocationContext<{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext};

{string.Join("\r\n", GenerateConstructors())}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(symbolVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}
}}

namespace LightMock.Generator
{{
    using global::{@namespace};

    public static partial class MockExtensions
    {{
        [DebuggerStepThrough]
        public static MockContext<global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}> Protected{typeArgumentsWithBrackets}(this IProtectedContext<{baseName}{typeArgumentsWithBrackets}> @this)
            {whereClause}
            => (MockContext<global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}{typeArgumentsWithBrackets}>)@this.{nameof(IProtectedContext<object>.ProtectedContext)};
    }}
}}
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

        public override void DoGeneratePart_GetInstanceType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return typeof(global::{@namespace}.{Prefix.MockClass}{className}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());"
                : $"if (contextType == typeof(global::{@namespace}.{baseName})) return typeof(global::{@namespace}.{Prefix.MockClass}{className});";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetProtectedContextType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $@"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments()));"
                : $@"if (contextType == typeof(global::{@namespace}.{baseName})) return MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.ProtectedToPublicInterface}{className}));";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetPropertiesContextType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $@"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{className}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments()));"
                : $@"if (contextType == typeof(global::{@namespace}.{baseName})) return MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{className}));";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetAssertType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return typeof(global::{@namespace}.{Prefix.AssertImplementation}{className}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());"
                : $"if (contextType == typeof(global::{@namespace}.{baseName})) return typeof(global::{@namespace}.{Prefix.AssertImplementation}{className});";
            here.Append(toAppend);
        }

        static IEnumerable<INamedTypeSymbol> GetAllBaseTypes(INamedTypeSymbol type)
        {
            for (var bt = type.BaseType; bt != null; bt = bt.BaseType)
                yield return bt;
        }
    }
}
