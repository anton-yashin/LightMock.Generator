using LightMock.Generator.Tests.Friends;
using LightMock.Generator.Tests.TestAbstractions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class Friends_Tests : GeneratorTestsBase
    {
        const int KExpected = 42;

        public Friends_Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void SameAssembly()
        {
            var testScript = LoadAssembly<IFoo>();

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void FriendAssembly()
        {
            const string baseClassName = nameof(SameAssembly);
            var baseAssemblyImage = GetAssemblyImage(baseClassName, Enumerable.Empty<MetadataReference>());

            var baseStream = new MemoryStream(baseAssemblyImage);
            var baseContext = new AssemblyLoadContext(baseClassName);
            var baseAssembly = baseContext.LoadFromStream(baseStream);
            baseStream.Position = 0;
            var baseMetadata = MetadataReference.CreateFromStream(baseStream);

            const string friendAssemblyName = nameof(FriendAssembly);
            var friendAssemblyImage = GetAssemblyImage(friendAssemblyName, Enumerable.Repeat(baseMetadata, 1));
            var friendAssemblyStream = new MemoryStream(friendAssemblyImage);
            var friendAssemblyContext = new AssemblyLoadContext(friendAssemblyName);
            var friendAssembly = baseContext.LoadFromStream(friendAssemblyStream);

            var testScript = FindTestScript<IFoo>(friendAssemblyName, friendAssembly);

            Assert.Equal(KExpected, testScript.DoRun());
        }

        byte[] GetAssemblyImage(string testClassName, IEnumerable<MetadataReference> links)
        {
            var (diagnostics, success, assembly) = DoCompileResource(links, testClassName);
            Assert.True(success);
            Assert.Empty(diagnostics);
            return assembly;
        }

        protected override string GetFullResourceName(string resourceName)
            => "Friends." + resourceName + ".test.cs";
    }
}
