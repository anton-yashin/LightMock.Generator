using LightMock.Generator.Tests.TestAbstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
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
            var name = nameof(DisableCodeGeneration);
            if (enable)
                name = name.Replace("Disable", "Enable");
            var compilation = CreateCompilation(Utils.LoadResource(
                GetFullResourceName(name)), name + Suffix.CSharpFile);
            var driver = CreateGenerationDriver(compilation);
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

        protected override string GetFullResourceName(string resourceName)
            => "Attributes." + resourceName + ".test.cs";
    }
}
