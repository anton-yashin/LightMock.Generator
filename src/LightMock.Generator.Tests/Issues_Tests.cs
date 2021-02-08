using LightMock.Generator.Tests.TestAbstractions;
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

        protected override string GetFullResourceName(string resourceName)
            => "Issues." + resourceName + ".test.cs";
    }
}
