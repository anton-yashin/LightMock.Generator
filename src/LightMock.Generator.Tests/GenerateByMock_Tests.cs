using LightMock.Generator.Tests.Mock;
using LightMock.Generator.Tests.Mock.EventNamespace2;
using LightMock.Generator.Tests.Mock.Namespace1;
using LightMock.Generator.Tests.Mock.Namespace2;
using Microsoft.CodeAnalysis;
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
    public class GenerateByMock_Tests : TestsBase
    {
        const int KExpected = 42;

        public GenerateByMock_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Fact]
        public void InterfaceWithBasicMethods()
        {
            const string KClassName = "InterfaceWithBasicMethods";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IInterfaceWithBasicMethods>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            context.Arrange(f => f.ReturnSomething()).Returns(4567);
            Assert.Equal(expected: 4567, mock.ReturnSomething());
        }

        [Fact]
        public void AbstractClassWithBasicMethods()
        {
            const string KClassName = "AbstractClassWithBasicMethods";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<AAbstractClassWithBasicMethods>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.GetSomething()).Returns(1234);
            Assert.Equal(expected: 1234, mock.GetSomething());

            Assert.Throws<NotImplementedException>(() => mock.NonAbstractNonVirtualMethod());

            mock.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void InterfaceWithBasicProperty()
        {
            const string KClassName = "InterfaceWithBasicProperty";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IInterfaceWithBasicProperty>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, mock.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 5678;
            Assert.Equal(5678, mock.GetAndSet);
        }

        [Fact]
        public void AbstractClassWithBasicProperty()
        {
            const string KClassName = "AbstractClassWithBasicProperty";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<AAbstractClassWithBasicProperty>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, mock.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 5678;
            Assert.Equal(5678, mock.GetAndSet);

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void InterfaceWithGenericMethod()
        {
            const string KClassName = "InterfaceWithGenericMethod";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IInterfaceWithGenericMethod>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.GenericReturn<int>()).Returns(1234);
            Assert.Equal(1234, mock.GenericReturn<int>());

            mock.GenericParam<int>(5678);
            context.Assert(f => f.GenericParam<int>(5678));

            var p = new object();
            mock.GenericWithConstraint(p);
            context.Assert(f => f.GenericWithConstraint(p));
        }


        [Fact]
        public void AbstractClassWithGenericMethod()
        {
            const string KClassName = "AbstractClassWithGenericMethod";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<AAbstractClassWithGenericMethod>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.GenericReturn<int>()).Returns(1234);
            Assert.Equal(1234, mock.GenericReturn<int>());

            mock.GenericParam<int>(5678);
            context.Assert(f => f.GenericParam<int>(5678));

            var p = new object();
            mock.GenericWithConstraint(p);
            context.Assert(f => f.GenericWithConstraint(p));

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericInterface()
        {
            const string KClassName = "GenericInterface";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IGenericInterface<int>>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            context.Arrange(f => f.GetSomething()).Returns(5678);
            Assert.Equal(5678, mock.GetSomething());

            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, mock.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 3456;
            Assert.Equal(3456, mock.GetAndSet);
        }

        [Fact]
        public void InterfaceWithMultipleNamespaces()
        {
            const string KClassName = "InterfaceWithMultipleNamespaces";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IInterfaceWithMultipleNamespaces>(KClassName, assembly, KClassName);
            var context = testScript.Context;
            var mock = testScript.MockObject;

            var arg1 = new MultipleNamespacesArgument();
            mock.DoSomething(arg1);
            context.Assert(f => f.DoSomething(arg1));

            var arg2 = new MultipleNamespacesArgument();
            context.Arrange(f => f.GetSomething()).Returns(arg2);
            Assert.Same(expected: arg2, mock.GetSomething());

            var arg3 = new MultipleNamespacesArgument();
            context.ArrangeProperty(f => f.SomeProperty);
            mock.SomeProperty = arg3;
            Assert.Same(expected: arg3, mock.SomeProperty);
        }

        [Fact]
        public void InterfaceWithEventSource()
        {
            const string KClassName = "InterfaceWithEventSource";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IInterfaceWithEventSource>(KClassName, assembly, KClassName);
            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void InterfaceWithEventSourceAndMultipleNamespaces()
        {
            const string KClassName = "InterfaceWithEventSourceAndMultipleNamespaces";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IInterfaceWithEventSourceAndMultipleNamespaces>(KClassName, assembly, KClassName);
            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericInterfaceWithGenericEvent()
        {
            const string KClassName = "GenericInterfaceWithGenericEvent";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var testScript = LoadAssembly<IGenericInterfaceWithGenericEvent<int>>(KClassName, assembly, KClassName);
            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        private (ImmutableArray<Diagnostic> diagnostics, bool succes, byte[] assembly) DoCompileResource(string resourceName)
        {
            return DoCompile(Utils.LoadResource("Mock." + resourceName + ".class.cs"));
        }

        private static ITestScript<T> LoadAssembly<T>(string KClassName, byte[] assembly, string className)
            where T : class
        {
            var alc = new AssemblyLoadContext(className);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var testClassType = loadedAssembly.ExportedTypes.Where(t => t.Name == KClassName).First();
            var testClass = Activator.CreateInstance(testClassType) ?? throw new InvalidOperationException("can't create test class");
            return (ITestScript<T>)testClass;
        }


    }
}
