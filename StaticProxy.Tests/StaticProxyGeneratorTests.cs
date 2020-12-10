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
        const string KClassName = "SomeClass";
        const string KFileName = KClassName + ".cs";

        [Fact]
        public void RunGenerator()
        {
            // prepare
            var resource = Utils.LoadResource(KFileName);
            var compilation = CreateCompilation(resource);
            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new StaticProxyGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

            // act
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

            // verify
            var result = updatedCompilation.Emit(new MemoryStream());
            Assert.True(result.Success);
            Assert.Empty(diagnostics);
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
                    MetadataReference.CreateFromFile(typeof(LightMock.InvocationInfo).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq.Expressions")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                },
                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }
}
