using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests.TestAbstractions
{
    public abstract class GeneratorTestsBase : TestsBase
    {
        protected GeneratorTestsBase(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper)
        {
        }

        protected abstract string GetFullResourceName(string resourceName);

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompileResource([CallerMemberName] string resourceName = "")
        {
            var fn = GetFullResourceName(resourceName);
            return DoCompile(Utils.LoadResource(fn), fn);
        }


        protected ITestScript<T> LoadAssembly<T>([CallerMemberName] string resourceName = "", string? className = null)
            where T : class
        {
            var (diagnostics, success, assembly) = DoCompileResource(resourceName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            className ??= resourceName;

            var alc = new AssemblyLoadContext(resourceName);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var testClassType = loadedAssembly.ExportedTypes.Where(t => t.Name == className).First();
            if (testClassType.ContainsGenericParameters)
                testClassType = testClassType.MakeGenericType(typeof(T).GetGenericArguments());
            var testClass = Activator.CreateInstance(testClassType) ?? throw new InvalidOperationException("can't create test class");
            return (ITestScript<T>)testClass;
        }


    }
}
