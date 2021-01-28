using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public abstract class TestsBase
    {
        protected readonly ITestOutputHelper testOutputHelper;

        public TestsBase(ITestOutputHelper testOutputHelper)
            => this.testOutputHelper = testOutputHelper;

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompile(string source, string compilationName)
        {
            var compilation = CreateCompilation(source, compilationName);
            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new LightMockGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var ms = new MemoryStream();
            var result = updatedCompilation.Emit(ms);
            foreach (var i in result.Diagnostics)
                testOutputHelper.WriteLine(i.ToString());
            return (diagnostics, result.Success, ms.ToArray());
        }


        protected static CSharpCompilation CreateCompilation(string source, string compilationName)
            => CSharpCompilation.Create(compilationName,
                syntaxTrees: new[]
                {
                    CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))
                },
                references: new[]
                {
                    MetadataReference.CreateFromFile(Assembly.GetCallingAssembly().Location),
                    MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(LightMock.InvocationInfo).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IMock<>).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Xunit.Assert).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq.Expressions")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                },
                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));

    }
}
