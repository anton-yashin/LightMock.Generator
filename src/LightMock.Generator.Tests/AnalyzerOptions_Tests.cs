using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class AnalyzerOptions_Tests : TestsBase
    {
        public AnalyzerOptions_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory, InlineData(true), InlineData(false)]
        public void DisableCodeGeneration(bool enable)
        {
            var compilation = CreateCompilation(@"/* nothing */", "test.cs");
            var options = new Dictionary<string, string>()
            {
                {GlobalOptionsNames.Enable, enable.ToString()}
            }.ToImmutableDictionary();
            var op = new MockAnalyzerConfigOptionsProvider(new MockAnalyzerConfigOptions(options));
            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new LightMockGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options, op);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var ms = new MemoryStream();
            var result = updatedCompilation.Emit(ms);
            Assert.True(result.Success);
            ms.Position = 0;
            var alc = new AssemblyLoadContext(nameof(DisableCodeGeneration));
            var loadedAssembly = alc.LoadFromStream(ms);
            var mockType = typeof(Mock<>);
            var mock_T_IsGenerated = loadedAssembly.ExportedTypes.Where(
                t => t.Name == mockType.Name && t.Namespace == mockType.Namespace).Any();
            Assert.Equal(expected: enable, actual: mock_T_IsGenerated);
        }
    }
}
