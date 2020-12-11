using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace StaticProxy.Tests
{
    public class StaticProxyGeneratorTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public StaticProxyGeneratorTests(ITestOutputHelper testOutputHelper)
            => this.testOutputHelper = testOutputHelper;

        [Fact]
        public void BasicFunction()
        {
            var (compilation, diagnostics, success) = DoCompile(Utils.LoadResource("BasicFunction.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);
        }

        [Fact]
        public void BasicProperty()
        {
            var (compilation, diagnostics, success) = DoCompile(Utils.LoadResource("BasicProperty.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);
        }

        [Fact]
        public void NoPartialKeyworkError()
        {
            var (compilation, diagnostics, success) = DoCompile(Utils.LoadResource("NoPartialKeyworkError.cs"));

            // verify
            Assert.False(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG002");
        }

        [Fact]
        public void NoInterfaceError()
        {
            var (compilation, diagnostics, success) = DoCompile(Utils.LoadResource("NoInterfaceError.cs"));

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG003");
        }

        [Fact]
        public void TooManyInterfacesWarning()
        {
            var (compilation, diagnostics, success) = DoCompile(Utils.LoadResource("TooManyInterfacesWarning.cs"));

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG004");
        }

        private (CSharpCompilation updated, ImmutableArray<Diagnostic> diagnostics, bool succes) DoCompile(string source)
        {
            var compilation = CreateCompilation(source);
            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new StaticProxyGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var result = updatedCompilation.Emit(new MemoryStream());
            foreach (var i in result.Diagnostics)
                testOutputHelper.WriteLine(i.ToString());
            return ((CSharpCompilation)updatedCompilation, diagnostics, result.Success);
        }


        private static CSharpCompilation CreateCompilation(string source, string? compilationName = null)
            => CSharpCompilation.Create(compilationName ?? Guid.NewGuid().ToString("N"),
                syntaxTrees: new[]
                {
                    CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))
                },
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(LightMock.InvocationInfo).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq.Expressions")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                },
                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }
}
