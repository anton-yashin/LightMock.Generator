using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;

namespace LightMock.Generator
{
    record struct CodeGenerationContext(
#if ROSLYN_4
        SourceProductionContext Context,
#else
        GeneratorExecutionContext Context,
#endif
        CSharpCompilation Compilation,
        CSharpParseOptions ParseOptions,
        CompilationContext CompilationContext)
    {
        public void ReportDiagnostic(Diagnostic diagnostic) 
            => Context.ReportDiagnostic(diagnostic);

        public void AddSource(string hint, SourceText text)
            => Context.AddSource(hint, text);

        public CodeGenerationContext Update(CSharpCompilation compilation)
            => new CodeGenerationContext(Context, compilation, ParseOptions, CompilationContext);

        public bool EmitDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            bool haveIssues = false;
            foreach (var d in diagnostics)
            {
                haveIssues = true;
                ReportDiagnostic(d);
            }
            return haveIssues;
        }

        public CodeGenerationContext AddSyntaxTrees(params SyntaxTree[] trees)
        {
            return Update(Compilation.AddSyntaxTrees(trees));
        }

        public CodeGenerationContext UpdateFromCompilationContext()
            => Update(CompilationContext.Update(Compilation));
    }
}
