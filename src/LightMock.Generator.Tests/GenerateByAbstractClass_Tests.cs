﻿using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;
using LightMock.Generator.Tests.AbstractClass;

namespace LightMock.Generator.Tests
{
    public class GenerateByAbstractClass_Tests : TestsBase
    {
        public GenerateByAbstractClass_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) 
        { }

        [Fact]
        public void BasicMethod()
        {
            const string KClassName = "BasicMethod";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = "ConcreteClass";
            var alc = new AssemblyLoadContext(className);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var concrete = loadedAssembly.ExportedTypes.Where(t => t.Name == className).First();
            if (concrete.ContainsGenericParameters)
                concrete = concrete.MakeGenericType(typeof(BasicMethod).GetGenericArguments().First());
            var generatedInterfaceType = loadedAssembly.ExportedTypes.Where(t => t.Name == "IP2P_" + KClassName).First();
            if (generatedInterfaceType.ContainsGenericParameters)
                throw new NotImplementedException("FIXME");
            var protectedContextType = typeof(MockContext<>).MakeGenericType(generatedInterfaceType);
            var protectedContext = Activator.CreateInstance(protectedContextType) ?? throw new InvalidOperationException("can't create protected context instance");
            var context = new MockContext<BasicMethod>();
            var mockInstance = Activator.CreateInstance(concrete, context, protectedContext) ?? throw new InvalidOperationException("can't create instance");
            var baseClass = (BasicMethod)mockInstance;
            var testClassType = loadedAssembly.ExportedTypes.Where(t => t.Name == className + "Test").First();
            dynamic testClass = Activator.CreateInstance(testClassType, mockInstance, protectedContext) ?? throw new InvalidOperationException("can't create test class");

            context.Arrange(f => f.GetSomething()).Returns(1234);
            Assert.Equal(expected: 1234, baseClass.GetSomething());

            testClass.TestProtectedMembers();
        }

        [Fact]
        public void NoPartialKeyworkError()
        {
            var (diagnostics, success, assembly) = DoCompileResource("NoPartialKeyworkError");

            // verify
            Assert.False(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG002");
        }

        private (ImmutableArray<Diagnostic> diagnostics, bool succes, byte[] assembly) DoCompileResource(string resourceName)
        {
            return DoCompile(Utils.LoadResource("AbstractClass." + resourceName + ".class.cs"));
        }
    }
}
