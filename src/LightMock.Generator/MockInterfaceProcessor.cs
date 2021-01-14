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
        const string KMockPrefix = "Mock_";
        private readonly SymbolVisitor<string> symbolVisitor;
        private readonly string className;
        private readonly string interfaceName;
        private readonly string typeArgumentsWithBrackets;
        private readonly string typeArgumentsWithComma;
        private readonly string commaArguments;
        private readonly string @namespace;

        public MockInterfaceProcessor(
            CSharpCompilation compilation,
            INamedTypeSymbol typeSymbol) : base(typeSymbol)
        {
            this.symbolVisitor = new InterfaceSymbolVisitor(compilation.Options.NullableContextOptions);
            var typeArguments = string.Join(",", typeSymbol.OriginalDefinition.TypeArguments.Select(i => i.Name));

            className = Prefix.MockClass + typeSymbol.Name;
            interfaceName = typeSymbol.Name;
            typeArgumentsWithBrackets = typeArguments.Length > 0 ? "<" + typeArguments + ">" : "";
            typeArgumentsWithComma = typeArguments.Length > 0 ? typeArguments + ", " : "";
            commaArguments = string.Join(",", typeSymbol.OriginalDefinition.TypeArguments.Select(i => " "));
            @namespace = typeSymbol.ContainingNamespace.ToDisplayString(KNamespaceDisplayFormat);
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
    {{
        private readonly IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context};

        public {className}(IInvocationContext<{interfaceName}{typeArgumentsWithBrackets}> {VariableNames.Context})
        {{
            this.{VariableNames.Context} = {VariableNames.Context};
        }}

        {string.Join("\r\n        ", members.Select(i => i.OriginalDefinition.Accept(symbolVisitor)))}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public override void DoGeneratePart_CreateMockInstance(StringBuilder here)
        {
            var toAppend = typeSymbol.IsGenericType 
                ? $"if (gtd == typeof({@namespace}.{interfaceName}<{commaArguments}>)) return (T)ActivateMockInstance(typeof({@namespace}.{className}<{commaArguments}>));" 
                : $"if (contextType == typeof({@namespace}.{interfaceName})) return (T)(object)new {@namespace}.{className}((MockContext<{@namespace}.{interfaceName}>)(object)this);";
            here.Append(toAppend);
        }

        public override string FileName => "Mock_" + base.FileName;
    }
}
