using LightMock.Generator.Tests.Issues;
using LightMock.Generator.Tests.TestAbstractions;
using System.Linq;
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

        protected override string GetFullResourceName(string resourceName)
            => "Issues." + resourceName + ".test.cs";
    }
}
