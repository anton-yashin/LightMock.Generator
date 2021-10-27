using LightMock.Generator.Tests.Mocks;
using LightMock.Generator.Tests.TestAbstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class AnalyzerOptions_Tests : GeneratorTestsBase
    {
        public AnalyzerOptions_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory, InlineData(true), InlineData(false)]
        public void DisableCodeGeneration(bool enable)
        {
            var compilation = CreateCompilation(
                Utils.LoadResource(GetFullResourceName(
                    nameof(DisableCodeGeneration))), nameof(DisableCodeGeneration) + Suffix.CSharpFile);
            var options = new Dictionary<string, string>()
            {
                {GlobalOptionsNames.Enable, enable.ToString()}
            }.ToImmutableDictionary();
            var op = new MockAnalyzerConfigOptionsProvider(new MockAnalyzerConfigOptions(options));
            var driver = CreateGenerationDriver(compilation, op);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var ms = new MemoryStream();
            var result = updatedCompilation.Emit(ms);
            Assert.True(result.Success);
            ms.Position = 0;
            var alc = new AssemblyLoadContext(nameof(DisableCodeGeneration));
            var loadedAssembly = alc.LoadFromStream(ms);
            var mockIsGenerated = loadedAssembly.ExportedTypes.Where(t => t.Name == "Property_IDisableCodeGeneration").Any();
            Assert.Equal(expected: enable, actual: mockIsGenerated);
        }

        protected override string GetFullResourceName(string resourceName) => nameof(AnalyzerOptions) + "." + resourceName + ".test.cs";
    }
}
