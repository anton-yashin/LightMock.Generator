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
using System.Runtime.CompilerServices;

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
            var (context, @interface) = LoadAssembly<IBasicMethod>();

            @interface.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));
            
            context.Arrange(f => f.ReturnSomething()).Returns(5678);
            Assert.Equal(5678, @interface.ReturnSomething());
        }

        [Fact]
        public void BasicProperty()
        {
            var (context, @interface) = LoadAssembly<IBasicProperty>();

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, @interface.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            @interface.GetAndSet = 5678;
            Assert.Equal(5678, @interface.GetAndSet);
        }

        [Fact]
        public void GenericMethod()
        {
            var (context, @interface) = LoadAssembly<IGenericMethod>();

            context.Arrange(f => f.GenericReturn<int>()).Returns(1234);
            Assert.Equal(1234, @interface.GenericReturn<int>());

            @interface.GenericParam<int>(5678);
            context.Assert(f => f.GenericParam<int>(5678));

            var p = new object();
            @interface.GenericWithClassConstraint(p);
            context.Assert(f => f.GenericWithClassConstraint(p));

            @interface.GenericWithStructConstraint<int>(1234);
            context.Assert(f => f.GenericWithStructConstraint<int>(1234));

            @interface.GenericWithConstraint(1234);
            context.Assert(f => f.GenericWithConstraint(1234));

            @interface.GenericWithManyConstraints<object, int, long>(p, 123, 456);
            context.Assert(f => f.GenericWithManyConstraints<object, int, long>(p, 123, 456));
        }

        [Fact]
        public void GenericClassAndGenericInterface()
        {
            const string resource = "GenericClassAndGenericInterface";
            const string className = resource + "`1";
            var (context, @interface) = LoadAssembly<IGenericClassAndGenericInterface<int>>(resource, className);

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
            var (context, @interface) = LoadAssembly<IMultipleNamespaces>();

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
            var (context, @interface) = LoadAssembly<IEventSource>();

            Assert.NotNull(context);
            Assert.NotNull(@interface);
        }

        [Fact]
        public void EventSourceMultipleNamespaces()
        {
            var (context, @interface) = LoadAssembly<IEventSourceMultipleNamespaces>();

            Assert.NotNull(context);
            Assert.NotNull(@interface);
        }

        [Fact]
        public void EventSourceGenericClass()
        {
            const string resource = "EventSourceGenericClass";
            const string className = resource + "`1";
            var (context, @interface) = LoadAssembly<IEventSourceGenericClass<GenerateByInterface_Tests>>(resource, className);

            Assert.NotNull(context);
            Assert.NotNull(@interface);
        }

        [Fact]
        public void NoPartialKeyworkError()
        {
            var (diagnostics, success, assembly) = DoCompileResource();

            // verify
            Assert.False(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG002");
        }

        [Fact]
        public void NoInterfaceError()
        {
            var (diagnostics, success, assembly) = DoCompileResource();

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG003");
        }

        [Fact]
        public void TooManyInterfacesWarning()
        {
            var (diagnostics, success, assembly) = DoCompileResource();

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG004");
        }

        [Fact]
        public void InherittedInterface()
        {
            var (context, @interface) = LoadAssembly<IInherittedInterface>();

            @interface.Foo();
            @interface.Bar();
            @interface.Baz();
            @interface.Quux();

            context.Assert(f => f.Foo());
            context.Assert(f => f.Bar());
            context.Assert(f => f.Baz());
            context.Assert(f => f.Quux());
        }

        private (MockContext<T>, T) LoadAssembly<T>([CallerMemberName] string resourceName = "", string? className = null)
        {
            var (diagnostics, success, assembly) = DoCompileResource(resourceName);
            Assert.True(success);
            Assert.Empty(diagnostics);

            className ??= resourceName;

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

        private (ImmutableArray<Diagnostic> diagnostics, bool succes, byte[] assembly) DoCompileResource([CallerMemberName] string resourceName = "")
        {
            return DoCompile(Utils.LoadResource("Interface." + resourceName + ".test.cs"), resourceName);
        }
    }
}
