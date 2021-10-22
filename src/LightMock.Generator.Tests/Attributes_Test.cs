using LightMock.Generator.Tests.TestAbstractions;
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
    public class Attributes_Test : GeneratorTestsBase
    {
        const int KExpected = 42;

        public Attributes_Test(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Theory, InlineData(false), InlineData(true)]
        public void DisableCodeGeneration(bool enable)
        {
            string code = enable
                ? "/* nothing */"
                : Utils.LoadResource(GetFullResourceName(nameof(DisableCodeGeneration)));

            var compilation = CreateCompilation(code, nameof(DisableCodeGeneration) + ".cs");
            var driver = CSharpGeneratorDriver.Create(
#if ROSLYN_4
                ImmutableArray.Create(new LightMockGenerator().AsSourceGenerator()),
#else
                ImmutableArray.Create(new LightMockGenerator()),
#endif
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

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

        protected override string GetFullResourceName(string resourceName)
            => "Attributes." + resourceName + ".test.cs";
    }
}
