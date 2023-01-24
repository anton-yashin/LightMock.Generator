using LightMock.Generator.Tests.Issues;
using LightMock.Generator.Tests.TestAbstractions;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class Issues_Tests : GeneratorTestsBase
    {
        const int KExpected = 42;

        public Issues_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Fact]
        public void NamespacesIssue()
        {
            var testScript = LoadAssembly<Playground.IFoo>();

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void HintNames()
        {
            var testScript = LoadAssembly<IHintNames>();

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        // Issue #47
        public void TypeOrNamespaceCouldNotBeFound()
        {
            var result = DoCompileResource();

            Assert.Empty(result.diagnostics.Where(
                d => d.Id == "CS1001" && d.Location.SourceTree?.FilePath.EndsWith(Suffix.FileName) == true));
        }

        [Fact]
        public void TypeKeyAttributeRemoved()
        {
            var (diagnostics, success, assembly) = DoCompileResource();
            Assert.True(success);
            Assert.Empty(diagnostics);

            var ms = new MemoryStream(assembly);
            var md = Mono.Cecil.ModuleDefinition.ReadModule(ms);
            foreach (var type in md.Types)
            {
                for (int i = 0; i < type.CustomAttributes.Count; i++)
                {
                    var attribute = type.CustomAttributes[i];
                    if (attribute.AttributeType.Name == nameof(TypeKeyAttribute))
                        type.CustomAttributes.RemoveAt(i--);
                }
            }
            var output = new MemoryStream();
            md.Write(output);
            output.Position = 0;

            const string testClassName = nameof(TypeKeyAttributeRemoved);
            var alc = new AssemblyLoadContext(testClassName);
            var loadedAssembly = alc.LoadFromStream(output);
            var script = FindTestScript<ITypeKeyAttributeRemoved>(testClassName, loadedAssembly);

            Assert.Equal(KExpected, script.DoRun());
        }

        protected override string GetFullResourceName(string resourceName)
            => "Issues." + resourceName + ".test.cs";
    }
}
