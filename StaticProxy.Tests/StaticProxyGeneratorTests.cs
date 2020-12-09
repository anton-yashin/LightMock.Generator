using System;
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

        private static CSharpCompilation CreateCompilation(string source, string compilationName = "someCompilation")
            => CSharpCompilation.Create(compilationName,
                syntaxTrees: new[]
                {
                    CSharpSyntaxTree.ParseText(source)
                },
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IServiceProvider).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ITypeDescriptorContext).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ISupportInitialize).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(TypeConverterAttribute).Assembly.Location),
                },
                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }
}
