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
using System.Runtime.CompilerServices;
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
            var testScript = LoadAssembly<IInterfaceWithBasicMethods>();
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
            var testScript = LoadAssembly<AAbstractClassWithBasicMethods>();
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
            var testScript = LoadAssembly<IInterfaceWithBasicProperty>();
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
            var testScript = LoadAssembly<AAbstractClassWithBasicProperty>();
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
            var testScript = LoadAssembly<IInterfaceWithGenericMethod>();
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
            var testScript = LoadAssembly<AAbstractClassWithGenericMethod>();
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
            var testScript = LoadAssembly<IGenericInterface<int>>();
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
        public void GenericAbstractClass()
        {
            var testScript = LoadAssembly<AGenericAbstractClass<int>>();
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

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericMockAndGenericInterface()
        {
            const string resource = "GenericMockAndGenericInterface";
            const string className = resource + "`1";
            var testScript = LoadAssembly<IGenericMockAndGenericInterface<int>>(resource, className);
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

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericMockAndGenericAbstractClass()
        {
            const string resource = "GenericMockAndGenericAbstractClass";
            const string className = resource + "`1";

            var testScript = LoadAssembly<AGenericMockAndGenericAbstractClass<int>>(resource, className);
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

            Assert.Equal(KExpected, testScript.DoRun());
        }


        [Fact]
        public void InterfaceWithMultipleNamespaces()
        {
            var testScript = LoadAssembly<IInterfaceWithMultipleNamespaces>();
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
        public void AbstractClassWithMultipleNamespaces()
        {
            var testScript = LoadAssembly<AAbstractClassWithMultipleNamespaces>();
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

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void InterfaceWithEventSource()
        {
            var testScript = LoadAssembly<IInterfaceWithEventSource>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void AbstractClassWithEventSource()
        {
            var testScript = LoadAssembly<AAbstractClassWithEventSource>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void InterfaceWithEventSourceAndMultipleNamespaces()
        {
            var testScript = LoadAssembly<IInterfaceWithEventSourceAndMultipleNamespaces>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void AbstractClassWithEventSourceAndMultipleNamespaces()
        {
            var testScript = LoadAssembly<AAbstractClassWithEventSourceAndMultipleNamespaces>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericInterfaceWithGenericEvent()
        {
            var testScript = LoadAssembly<IGenericInterfaceWithGenericEvent<int>>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericAbstractClassWithGenericEvent()
        {
            var testScript = LoadAssembly<AGenericAbstractClassWithGenericEvent<int>>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void AbstractClassWithConstructors()
        {
            var testScript = LoadAssembly<AAbstractClassWithConstructors>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void TypeCachingWithInterface()
        {
            var testScript = LoadAssembly<ITypeCachingWithInterface>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void TypeCachingWithGenericInterface()
        {
            var testScript = LoadAssembly<ITypeCachingWithGenericInterface<int>>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void TypeCachingWithAbstractClass()
        {
            var testScript = LoadAssembly<ATypeCachingWithAbstractClass>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void TypeCachingWithGenericAbstractClass()
        {
            var testScript = LoadAssembly<ATypeCachingWithGenericAbstractClass<int>>();

            Assert.NotNull(testScript.Context);
            Assert.NotNull(testScript.MockObject);
            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        private (ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly) DoCompileResource(string resourceName)
        {
            var fn = "Mock." + resourceName + ".class.cs";
            return DoCompile(Utils.LoadResource(fn), fn);
        }

        private ITestScript<T> LoadAssembly<T>([CallerMemberName]string resourceName = "", string? className = null)
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
