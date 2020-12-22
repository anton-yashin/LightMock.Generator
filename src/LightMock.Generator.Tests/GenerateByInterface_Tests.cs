using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;
using LightMock.Generator.Tests.Interface.MultipleNamespaces2;
using LightMock.Generator.Tests.Interface.MultipleNamespaces1;
using EventNamespace2;
using LightMock.Generator.Tests.Interface;

namespace LightMock.Generator.Tests
{
    public class GenerateByInterface_Tests : TestsBase
    {

        public GenerateByInterface_Tests(ITestOutputHelper testOutputHelper)
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

            var (context, @interface) = LoadAssembly<IBasicMethod>(KClassName, assembly);

            @interface.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));
            
            context.Arrange(f => f.ReturnSomething()).Returns(5678);
            Assert.Equal(5678, @interface.ReturnSomething());
        }

        [Fact]
        public void BasicProperty()
        {
            const string KClassName = "BasicProperty";
            var (diagnostics, success, assembly) = DoCompileResource(KClassName);

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IBasicProperty>(KClassName, assembly);

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, @interface.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            @interface.GetAndSet = 5678;
            Assert.Equal(5678, @interface.GetAndSet);
        }

        [Fact]
        public void GenericMethod()
        {
            const string KClassName = "GenericMethod";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IGenericMethod>(KClassName, assembly);

            context.Arrange(f => f.GenericReturn<int>()).Returns(1234);
            Assert.Equal(1234, @interface.GenericReturn<int>());

            @interface.GenericParam<int>(5678);
            context.Assert(f => f.GenericParam<int>(5678));

            var p = new object();
            @interface.GenericWithConstraint(p);
            context.Assert(f => f.GenericWithConstraint(p));
        }

        [Fact]
        public void GenericClassAndGenericInterface()
        {
            const string KClassName = "GenericClassAndGenericInterface";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IGenericClassAndGenericInterface<int>>(KClassName + "`1", assembly);

            @interface.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            context.Arrange(f => f.GetSomething()).Returns(5678);
            Assert.Equal(5678, @interface.GetSomething());

            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, @interface.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            @interface.GetAndSet = 3456;
            Assert.Equal(3456, @interface.GetAndSet);
        }

        [Fact]
        public void MultipleNamespaces()
        {
            const string KClassName = "MultipleNamespaces";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IMultipleNamespaces>(KClassName, assembly);

            var arg1 = new MultipleNamespacesArgument();
            @interface.DoSomething(arg1);
            context.Assert(f => f.DoSomething(arg1));

            var arg2 = new MultipleNamespacesArgument();
            context.Arrange(f => f.GetSomething()).Returns(arg2);
            Assert.Same(expected: arg2, @interface.GetSomething());

            var arg3 = new MultipleNamespacesArgument();
            context.ArrangeProperty(f => f.SomeProperty);
            @interface.SomeProperty = arg3;
            Assert.Same(expected: arg3, @interface.SomeProperty);
        }

        [Fact]
        public void EventSource()
        {
            const string KClassName = "EventSource";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IEventSource>(KClassName, assembly);
            Assert.NotNull(context);
            Assert.NotNull(@interface);
        }

        [Fact]
        public void EventSourceMultipleNamespaces()
        {
            const string KClassName = "EventSourceMultipleNamespaces";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IEventSourceMultipleNamespaces>(KClassName, assembly);
            Assert.NotNull(context);
            Assert.NotNull(@interface);
        }

        [Fact]
        public void EventSourceGenericClass()
        {
            const string KClassName = "EventSourceGenericClass";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IEventSourceGenericClass<GenerateByInterface_Tests>>(KClassName + "`1", assembly);
            Assert.NotNull(context);
            Assert.NotNull(@interface);
        }

        [Fact]
        public void NoPartialKeyworkError()
        {
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource("NoPartialKeyworkError.class.cs"));

            // verify
            Assert.False(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG002");
        }

        [Fact]
        public void NoInterfaceError()
        {
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource("NoInterfaceError.class.cs"));

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG003");
        }

        [Fact]
        public void TooManyInterfacesWarning()
        {
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource("TooManyInterfacesWarning.class.cs"));

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG004");
        }

        private static (MockContext<T>, T) LoadAssembly<T>(string className, byte[] assembly)
        {
            var alc = new AssemblyLoadContext(className);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var concrete = loadedAssembly.ExportedTypes.Where(t => t.Name == className).First();
            if (concrete.ContainsGenericParameters)
                concrete = concrete.MakeGenericType(typeof(T).GetGenericArguments().First());
            var context = new MockContext<T>();
            var mockInstance = Activator.CreateInstance(concrete, context) ?? throw new InvalidOperationException("can't create instance");
            var @interface = (T)mockInstance;
            return (context, @interface);
        }

        private (ImmutableArray<Diagnostic> diagnostics, bool succes, byte[] assembly) DoCompileResource(string resourceName)
        {
            return DoCompile(Utils.LoadResource("Interface." + resourceName + ".class.cs"));
        }
    }
}
