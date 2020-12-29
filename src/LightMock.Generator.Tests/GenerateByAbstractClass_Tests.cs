using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;
using LightMock.Generator.Tests.AbstractClass;
using LightMock.Generator.Tests.AbstractClass.Namespace2;
using LightMock.Generator.Tests.AbstractClass.Namespace1;
using LightMock.Generator.Tests.AbstractClass.Namespace4;

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

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<ABasicMethod>(KClassName, assembly, className);

            context.Arrange(f => f.GetSomething()).Returns(1234);
            Assert.Equal(expected: 1234, baseClass.GetSomething());

            Assert.Throws<NotImplementedException>(() => baseClass.NonAbstractNonVirtualMethod());

            baseClass.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            testClass.TestProtectedMembers();
        }

        [Fact]
        public void BasicProperty()
        {
            const string KClassName = "BasicProperty";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<ABasicProperty>(KClassName, assembly, className);

            const int KExpected1 = 1234;
            context.Arrange(f => f.OnlyGet).Returns(KExpected1);
            Assert.Equal(expected: KExpected1, baseClass.OnlyGet);
            Assert.Throws<NotImplementedException>(() => baseClass.NonAbstractNonVirtualProperty);

            const int KExpected2 = 9218719;
            context.ArrangeProperty(f => f.GetAndSet);
            baseClass.GetAndSet = KExpected2;
            Assert.Equal(KExpected2, baseClass.GetAndSet);

            testClass.TestProtectedMembers();
        }

        [Fact]
        public void GenericMethod()
        {
            const string KClassName = "GenericMethod";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<AGenericMethod>(KClassName, assembly, className);

            context.Arrange(f => f.GenericReturn<int>()).Returns(1234);
            Assert.Equal(1234, baseClass.GenericReturn<int>());

            baseClass.GenericParam<int>(5678);
            context.Assert(f => f.GenericParam<int>(5678));

            var p = new object();
            baseClass.GenericWithConstraint(p);
            context.Assert(f => f.GenericWithConstraint(p));

            testClass.TestProtectedMembers();
        }

        [Fact]
        public void GenericClassAndGenericBaseClass()
        {
            const string KClassName = "GenericClassAndGenericBaseClass";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<AGenericClassAndGenericBaseClass<int>>(KClassName, assembly, className + "`1");

            baseClass.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            context.Arrange(f => f.GetSomething()).Returns(5678);
            Assert.Equal(5678, baseClass.GetSomething());

            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, baseClass.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            baseClass.GetAndSet = 3456;
            Assert.Equal(3456, baseClass.GetAndSet);

            testClass.TestProtectedMembers();
        }

        [Fact]
        public void MultipleNamespaces()
        {
            const string KClassName = "MultipleNamespaces";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<AMultipleNamespaces>(KClassName, assembly, className);

            var a1 = new AMultipleNamespacesArgument();
            baseClass.DoSomething(a1);
            context.Assert(f => f.DoSomething(a1));

            var a2 = new AMultipleNamespacesArgument();
            context.Arrange(f => f.GetSomething()).Returns(a2);
            Assert.Same(a2, baseClass.GetSomething());

            var a3 = new AMultipleNamespacesArgument();
            context.ArrangeProperty(f => f.SomeProperty);
            baseClass.SomeProperty = a3;
            Assert.Same(a3, baseClass.SomeProperty);

            testClass.TestProtectedMembers();
        }


        [Fact]
        public void EventSource()
        {
            const string KClassName = "EventSource";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<AEventSource>(KClassName, assembly, className);

            Assert.NotNull(baseClass);
        }

        [Fact]
        public void EventSourceMultipleNamespaces()
        {
            const string KClassName = "EventSourceMultipleNamespaces";

            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            string className = KClassName;
            var (context, baseClass, testClass) = LoadAssembly<AEventSourceMultipleNamespaces>(KClassName, assembly, className);

            var bt = baseClass.GetType();

            Assert.NotNull(baseClass);
        }


        private static (MockContext<T> context, T baseClass, dynamic testClass) LoadAssembly<T>(string KClassName, byte[] assembly, string className)
        {
            var alc = new AssemblyLoadContext(className);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var concrete = loadedAssembly.ExportedTypes.Where(t => t.Name == className).First();
            if (concrete.ContainsGenericParameters)
                concrete = concrete.MakeGenericType(typeof(T).GetGenericArguments().First());
            var generatedInterfaceType = loadedAssembly.ExportedTypes.Where(t => t.Name == "IP2P_A" + className).First();
            if (generatedInterfaceType.ContainsGenericParameters)
                generatedInterfaceType = generatedInterfaceType.MakeGenericType(typeof(T).GetGenericArguments().First());
            var protectedContextType = typeof(MockContext<>).MakeGenericType(generatedInterfaceType);
            var protectedContext = Activator.CreateInstance(protectedContextType) ?? throw new InvalidOperationException("can't create protected context instance");
            var context = new MockContext<T>();
            var mockInstance = Activator.CreateInstance(concrete, context, protectedContext) ?? throw new InvalidOperationException("can't create instance");
            var baseClass = (T)mockInstance;
            var testClassType = loadedAssembly.ExportedTypes.Where(t => t.Name == KClassName + "Test").First();
            var testClass = Activator.CreateInstance(testClassType, mockInstance, protectedContext) ?? throw new InvalidOperationException("can't create test class");
            return (context, baseClass, testClass);
        }

        [Fact]
        public void NoPartialKeyworkError()
        {
            var (diagnostics, success, assembly) = DoCompileResource("NoPartialKeyworkError");

            // verify
            Assert.False(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG002");
        }

        [Fact]
        public void TooManyInterfacesWarning()
        {
            var (diagnostics, success, assembly) = DoCompileResource("TooManyInterfacesWarning");

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG004");
        }

        private (ImmutableArray<Diagnostic> diagnostics, bool succes, byte[] assembly) DoCompileResource(string resourceName)
        {
            return DoCompile(Utils.LoadResource("AbstractClass." + resourceName + ".class.cs"));
        }
    }
}
