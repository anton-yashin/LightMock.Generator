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
    sealed class MockInterfaceProcessor : ClassProcessor
    {
        private readonly SymbolVisitor<string> symbolVisitor;
        private readonly string className;
        private readonly string interfaceName;
        private readonly string typeArgumentsWithBrackets;
        private readonly string commaArguments;
        private readonly string whereClause;
        private readonly string @namespace;

        public MockInterfaceProcessor(
            CSharpCompilation compilation,
            INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {
            this.symbolVisitor = new InterfaceSymbolVisitor(compilation.Options.NullableContextOptions);
            var to = typeSymbol.OriginalDefinition;
            var withTypeParams = to.ToDisplayString(SymbolDisplayFormats.WithTypeParams);
            var withWhereClause = to.ToDisplayString(SymbolDisplayFormats.WithWhereClause);
            var typeArguments = withTypeParams.Replace(to.ToDisplayString(SymbolDisplayFormats.Namespace), "");

            className = Prefix.MockClass + typeSymbol.Name;
            interfaceName = typeSymbol.Name;
            typeArgumentsWithBrackets = typeArguments.Length > 0 ? typeArguments : "";
            commaArguments = string.Join(",", to.TypeArguments.Select(i => " "));
            whereClause = withWhereClause.Replace(withTypeParams, "");
            @namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace);
        }

        public override IEnumerable<Diagnostic> GetErrors()
            => Enumerable.Empty<Diagnostic>();

        public override IEnumerable<Diagnostic> GetWarnings()
            => Enumerable.Empty<Diagnostic>();

        public override SourceText DoGenerate()
        {
            var members = typeSymbol.GetMembers();
            var code = $@"// <auto-generated />
using LightMock;

namespace {@namespace}
{{
    partial class {className}{typeArgumentsWithBrackets} : {interfaceName}{typeArgumentsWithBrackets}
        {whereClause}
    {{
        private readonly IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context};

        public {className}(IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
        }}

        public {className}(IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context}, object unused)
            : this({VariableNames.Context}) {{ }}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(symbolVisitor)))}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public override void DoGeneratePart_GetInstanceType(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType 
                ? $"if (gtd == typeof({@namespace}.{interfaceName}<{commaArguments}>)) return typeof({@namespace}.{className}<{commaArguments}>).MakeGenericType(contextType.GetGenericArguments());" 
                : $"if (contextType == typeof({@namespace}.{interfaceName})) return typeof({@namespace}.{className});";
            here.Append(toAppend);
        }

        public override string FileName => "Mock_" + base.FileName;
    }
}
