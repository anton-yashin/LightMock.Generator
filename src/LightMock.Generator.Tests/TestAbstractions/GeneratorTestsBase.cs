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
            return DoCompileResource(new string[] { resourceName }, Enumerable.Empty<MetadataReference>());
        }

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompileResource(
            IEnumerable<MetadataReference> linkAssemblies, [CallerMemberName] string resourceName = "")
        {
            return DoCompileResource(new string[] { resourceName }, linkAssemblies);
        }

        protected (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompileResource(
            IEnumerable<string> resourceNames, IEnumerable<MetadataReference> linkAssemblies)
        {
            return DoCompile(
                from i in resourceNames
                let fn = GetFullResourceName(i)
                select new TestableSourceText(Utils.LoadResource(fn), fn), linkAssemblies);
        }

        protected ITestScript<T> LoadAssembly<T>([CallerMemberName] string resourceName = "", string? testClassName = null)
            where T : class
        {
            return LoadAssembly<T>(new string[] { resourceName }, testClassName ?? resourceName);
        }

        protected ITestScript<T> LoadAssembly<T>(
            IEnumerable<string> resourceNames,
            [CallerMemberName] string testClassName = "")
            where T : class
        {
            return LoadAssembly<T>(resourceNames, Enumerable.Empty<MetadataReference>(), testClassName);
        }

        protected ITestScript<T> LoadAssembly<T>(
            IEnumerable<string> resourceNames,
            IEnumerable<MetadataReference> linkAssemblies,
            [CallerMemberName]string testClassName = "")
            where T : class
        {
            var (diagnostics, success, assembly) = DoCompileResource(resourceNames, linkAssemblies);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var alc = new AssemblyLoadContext(testClassName);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            return FindTestScript<T>(testClassName, loadedAssembly);
        }

        protected static ITestScript<T> FindTestScript<T>(
            string testClassName,
            System.Reflection.Assembly loadedAssembly)
            where T : class
        {
            var testClassType = loadedAssembly.ExportedTypes.Where(t => t.Name == testClassName).First();
            if (testClassType.ContainsGenericParameters)
                testClassType = testClassType.MakeGenericType(typeof(T).GetGenericArguments());
            var testClass = Activator.CreateInstance(testClassType) ?? throw new InvalidOperationException("can't create test class");
            return (ITestScript<T>)testClass;
        }

        protected static string[] Params(params string[] @params) => @params;
    }
}
