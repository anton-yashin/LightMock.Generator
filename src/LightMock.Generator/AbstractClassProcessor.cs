﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class AbstractClassProcessor : ClassProcessor
    {
        private readonly ClassDeclarationSyntax candidateClass;
        private readonly INamedTypeSymbol baseClass;
        private readonly NullableContextOptions nullableContextOptions;
        private readonly ProtectedMemberSymbolVisitor protectedVisitor;

        public AbstractClassProcessor(
            CSharpCompilation compilation,
            ClassDeclarationSyntax candidateClass,
            INamedTypeSymbol typeSymbol,
            INamedTypeSymbol baseClass) : base(typeSymbol)
        {
            this.candidateClass = candidateClass;
            this.baseClass = baseClass;
            this.nullableContextOptions = compilation.Options.NullableContextOptions;
            this.protectedVisitor = new ProtectedMemberSymbolVisitor(nullableContextOptions);
        }

        public override SourceText DoGenerate()
        {
            var className = typeSymbol.IsGenericType
                ? typeSymbol.Name + "<" + string.Join(",", typeSymbol.TypeParameters.Select(i => i.Name)) + ">"
                : typeSymbol.Name;
            var nameSpace = typeSymbol.ContainingNamespace.ToDisplayString(KNamespaceDisplayFormat);
            var symbolVisitor = new AbstractClassSymbolVisitor(nullableContextOptions, nameSpace);
            var baseName = baseClass.Accept(symbolVisitor);
            var members = baseClass.GetMembers();
            var interfaceName = "IP2P_" + (typeSymbol.IsGenericType 
                ? baseClass.Name + "<" + string.Join(",", typeSymbol.TypeParameters.Select(i => i.Name)) + ">"
                : baseClass.Name);
            var code = $@"// <auto-generated />
using LightMock;

namespace {nameSpace}
{{
    public interface {interfaceName}
    {{
        {string.Join("\r\n        ", members.Select(i => i.Accept(protectedVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}


    partial class {className} : {interfaceName}
    {{
        private readonly IInvocationContext<{baseName}> {VariableNames.Context};
        private readonly IInvocationContext<{interfaceName}> {VariableNames.ProtectedContext};

        public {typeSymbol.Name}(IInvocationContext<{baseName}> {VariableNames.Context}, IInvocationContext<{interfaceName}> {VariableNames.ProtectedContext})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
            this.{VariableNames.ProtectedContext} = {VariableNames.ProtectedContext};
        }}

        {string.Join("\r\n        ", members.Select(i => i.Accept(symbolVisitor)).SkipWhile(i => string.IsNullOrWhiteSpace(i)))}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public override IEnumerable<Diagnostic> GetErrors() => Enumerable.Empty<Diagnostic>();

        public override IEnumerable<Diagnostic> GetWarnings()
        {
            if (typeSymbol.Interfaces.Length > 0)
            {
                yield return Diagnostic.Create(
                    DiagnosticsDescriptors.KTooManyInterfacesWarningDescriptor,
                    Location.Create(candidateClass.SyntaxTree, new TextSpan()),
                    typeSymbol.Name);
            }
        }
    }
}
