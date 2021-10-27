using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests.TestAbstractions
{
    public abstract class TestsBase
    {
        protected readonly ITestOutputHelper testOutputHelper;

        public TestsBase(ITestOutputHelper testOutputHelper)
            => this.testOutputHelper = testOutputHelper;

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompile(string sourceCode, string hint)
            => DoCompile(new TestableSourceText[] { new TestableSourceText(sourceCode, hint) });

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompile(IEnumerable<TestableSourceText> texts)
        {
            var compilation = CreateCompilation(texts);
            var driver = CreateGenerationDriver(compilation);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var ms = new MemoryStream();
            var result = updatedCompilation.Emit(ms);
            foreach (var i in result.Diagnostics)
                testOutputHelper.WriteLine(i.ToString());
            return (diagnostics, result.Success, ms.ToArray());
        }

        protected GeneratorDriver CreateGenerationDriver(CSharpCompilation compilation, AnalyzerConfigOptionsProvider? analyzerConfigOptions = null)
        {
            GeneratorDriver result;
#if ROSLYN_4
            result = CSharpGeneratorDriver.Create(new LightMockGenerator())
                .WithUpdatedParseOptions(compilation.SyntaxTrees.First().Options);
            if (analyzerConfigOptions != null)
                result = result.WithUpdatedAnalyzerConfigOptions(analyzerConfigOptions);
#else
            result = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new LightMockGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options, analyzerConfigOptions);
#endif
            return result;

        }

        protected static CSharpCompilation CreateCompilation(string sourceCode, string hint)
            => CreateCompilation(new TestableSourceText[] { new TestableSourceText(sourceCode, hint) });

        protected static CSharpCompilation CreateCompilation(IEnumerable<TestableSourceText> texts)
            => CSharpCompilation.Create(
                
                assemblyName: texts.First().hint,

                syntaxTrees: texts.Select(i
                    => CSharpSyntaxTree.ParseText(i.sourceCode, new CSharpParseOptions(LanguageVersion.Preview),
                        Path.GetFullPath(i.hint))),
                
                references: new[]
                {
                    MetadataReference.CreateFromFile(Assembly.GetCallingAssembly().Location),
                    MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(MockContext<>).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IMock<>).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Xunit.Assert).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading.Tasks")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq.Expressions")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                },

                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }
}
