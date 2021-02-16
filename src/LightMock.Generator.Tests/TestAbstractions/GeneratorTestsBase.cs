using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
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

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompileResource(
            [CallerMemberName] string resourceName = "")
        {
            return DoCompileResource(new string[] { resourceName });
        }

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompileResource(
            IEnumerable<string> resourceNames)
        {
            return DoCompile(
                from i in resourceNames
                let fn = GetFullResourceName(i)
                select new TestableSourceText(Utils.LoadResource(fn), fn));
        }

        protected ITestScript<T> LoadAssembly<T>([CallerMemberName] string resourceName = "", string? testClassName = null)
            where T : class
        {
            return LoadAssembly<T>(new string[] { resourceName }, testClassName ?? resourceName);
        }

        protected ITestScript<T> LoadAssembly<T>(IEnumerable<string> resourceNames, [CallerMemberName]string testClassName = "")
            where T : class
        {
            var (diagnostics, success, assembly) = DoCompileResource(resourceNames);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var alc = new AssemblyLoadContext(testClassName);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var testClassType = loadedAssembly.ExportedTypes.Where(t => t.Name == testClassName).First();
            if (testClassType.ContainsGenericParameters)
                testClassType = testClassType.MakeGenericType(typeof(T).GetGenericArguments());
            var testClass = Activator.CreateInstance(testClassType) ?? throw new InvalidOperationException("can't create test class");
            return (ITestScript<T>)testClass;
        }

        static string[] Params(params string[] @params) => @params;
    }
}
