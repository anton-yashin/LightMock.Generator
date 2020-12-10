using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace StaticProxy.Tests
{
    public class StaticProxyGeneratorTests
    {
        [Fact]
        public void Test()
        {
            var resource = Utils.LoadResource("SomeClass.cs");
            var compilation = CreateCompilation(resource);
            Assert.NotEmpty(resource);
            Assert.NotNull(compilation);
        }

        [Fact]
        public void RunGenerator()
        {
            var resource = Utils.LoadResource("SomeClass.cs");
            var compilation = CreateCompilation(resource);
            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new StaticProxyGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            Assert.Empty(diagnostics);
            Assert.NotNull(updatedCompilation);
        }

        private static CSharpCompilation CreateCompilation(string source, string compilationName = "someCompilation")
            => CSharpCompilation.Create(compilationName,
                syntaxTrees: new[]
                {
                    CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))
                },
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IServiceProvider).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ITypeDescriptorContext).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ISupportInitialize).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(TypeConverterAttribute).Assembly.Location)
                },
                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }
}
