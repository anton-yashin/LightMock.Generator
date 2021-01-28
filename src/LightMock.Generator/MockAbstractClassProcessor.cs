using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class MockAbstractClassProcessor : ClassProcessor
    {
        private readonly SymbolVisitor<string> protectedVisitor;
        private readonly SymbolVisitor<string> propertyDefinitionVisitor;
        private readonly SymbolVisitor<string> assertImplementationVisitor;
        private readonly string @namespace;
        private readonly SymbolVisitor<string> symbolVisitor;
        private readonly string baseName;
        private readonly string className;
        private readonly string interfaceName;
        private readonly string typeArgumentsWithBrackets;
        private readonly string whereClause;
        private readonly string commaArguments;
        private readonly List<string> constructors;
        private readonly List<string> constructorsCall;
        private readonly SyntaxNode containingGeneric;

        public MockAbstractClassProcessor(
            CSharpCompilation compilation,
            SyntaxNode containingGeneric,
            INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {

            this.protectedVisitor = new ProtectedMemberSymbolVisitor();
            this.propertyDefinitionVisitor = new PropertyDefinitionVisitor();
            this.assertImplementationVisitor = new AssertImplementationVisitor(
                compilation.Options.NullableContextOptions,
                SymbolDisplayFormats.AbstractClass);
            this.@namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);
            this.interfaceName = Prefix.ProtectedToPublicInterface + typeSymbol.Name;
            this.symbolVisitor = new MockAbstractClassSymbolVisitor(@namespace, interfaceName);
            this.baseName = typeSymbol.OriginalDefinition.Name;
            this.className = Prefix.MockClass + typeSymbol.Name;

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
        public {className}(IInvocationContext<{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext},
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
        public {className}(IInvocationContext<{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context},
            IInvocationContext<{Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext},
            IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext})
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
        public {Prefix.AssertImplementation}{baseName}(
            IMockContext<{Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context},
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
        public {Prefix.AssertImplementation}{baseName}(
            IMockContext<{Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context},
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
    public interface {Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}
    {{
        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(propertyDefinitionVisitor)))}
    }}

    sealed class {Prefix.AssertImplementation}{baseName}{typeArgumentsWithBrackets} : {baseName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IMockContext<{Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly Invoked {VariableNames.Invoked};

{string.Join("\r\n", GenerateAssertConstructors())}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(assertImplementationVisitor)))}
    }}

    public interface {interfaceName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(protectedVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}


    partial class {className}{typeArgumentsWithBrackets} : {baseName}{typeArgumentsWithBrackets}, {interfaceName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IInvocationContext<{baseName}{typeArgumentsWithBrackets}> {VariableNames.Context};
        private readonly IInvocationContext<{Prefix.PropertyToFuncInterface}{baseName}{typeArgumentsWithBrackets}> {VariableNames.PropertiesContext};
        private readonly IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.ProtectedContext};

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
        public static MockContext<global::{@namespace}.{@interfaceName}{typeArgumentsWithBrackets}> Protected{typeArgumentsWithBrackets}(this IProtectedContext<{baseName}{typeArgumentsWithBrackets}> @this)
            {whereClause}
            => (MockContext<global::{@namespace}.{@interfaceName}{typeArgumentsWithBrackets}>)@this.{nameof(IProtectedContext<object>.ProtectedContext)};
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
                ? $"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return typeof(global::{@namespace}.{className}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());"
                : $"if (contextType == typeof(global::{@namespace}.{baseName})) return typeof(global::{@namespace}.{className});";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetProtectedContextType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $@"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return MockDefaults.MockContextType.MakeGenericType(typeof(global::{@namespace}.{interfaceName}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments()));"
                : $@"if (contextType == typeof(global::{@namespace}.{baseName})) return MockDefaults.MockContextType.MakeGenericType(typeof(global::{@namespace}.{interfaceName}));";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetPropertiesContextType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $@"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return MockDefaults.MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{baseName}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments()));"
                : $@"if (contextType == typeof(global::{@namespace}.{baseName})) return MockDefaults.MockContextType.MakeGenericType(typeof(global::{@namespace}.{Prefix.PropertyToFuncInterface}{baseName}));";
            here.Append(toAppend);
        }

        public override void DoGeneratePart_GetAssertType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType
                ? $"if (gtd == typeof(global::{@namespace}.{baseName}<{commaArguments}>)) return typeof(global::{@namespace}.{Prefix.AssertImplementation}{baseName}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());"
                : $"if (contextType == typeof(global::{@namespace}.{baseName})) return typeof(global::{@namespace}.{Prefix.AssertImplementation}{baseName});";
            here.Append(toAppend);
        }

        public override string FileName => Prefix.MockClass + base.FileName;
    }
}
