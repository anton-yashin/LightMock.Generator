using LightMock.Generator.Tests.AbstractClass;
using LightMock.Generator.Tests.AbstractClass.EventNamespace2;
using LightMock.Generator.Tests.AbstractClass.Namespace1;
using LightMock.Generator.Tests.AbstractClass.Namespace2;
using LightMock.Generator.Tests.TestAbstractions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class AbstractClass_Tests : GeneratorTestsBase
    {
        const int KExpected = 42;

        public AbstractClass_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

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
        public void AbstractClassWithBasicProperty()
        {
            var testScript = LoadAssembly<AAbstractClassWithBasicProperty>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 5678));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet));

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, mock.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 5678;
            Assert.Equal(5678, mock.GetAndSet);
            context.AssertSet(f => f.GetAndSet = 5678);
            context.AssertGet(f => f.GetAndSet);

            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 1234));

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 5678, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Never));

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Exactly(2)));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 5678, Invoked.Exactly(2)));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Exactly(2)));

            Assert.Equal(KExpected, testScript.DoRun());
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
            mock.GenericWithClassConstraint(p);
            context.Assert(f => f.GenericWithClassConstraint(p));

            mock.GenericWithStructConstraint<int>(1234);
            context.Assert(f => f.GenericWithStructConstraint<int>(1234));

            mock.GenericWithConstraint(1234);
            context.Assert(f => f.GenericWithConstraint(1234));

            mock.GenericWithManyConstraints<object, int, long>(p, 123, 456);
            context.Assert(f => f.GenericWithManyConstraints<object, int, long>(p, 123, 456));

            Assert.Equal(KExpected, testScript.DoRun());
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

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet));
            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, mock.OnlyGet);
            context.AssertGet(f => f.OnlyGet);
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Exactly(2)));

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 3456));
            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 3456;
            Assert.Equal(3456, mock.GetAndSet);
            context.AssertGet(f => f.GetAndSet);
            context.AssertSet(f => f.GetAndSet = 3456);
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 1234));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 3456, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Exactly(2)));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 3456, Invoked.Exactly(2)));

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

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet));
            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, mock.OnlyGet);
            context.AssertGet(f => f.OnlyGet);
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Exactly(2)));

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 3456));
            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 3456;
            Assert.Equal(3456, mock.GetAndSet);
            context.AssertGet(f => f.GetAndSet);
            context.AssertSet(f => f.GetAndSet = 3456);
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 1234));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 3456, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Exactly(2)));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 3456, Invoked.Exactly(2)));

            Assert.Equal(KExpected, testScript.DoRun());
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
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.SomeProperty));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.SomeProperty = arg3));
            context.ArrangeProperty(f => f.SomeProperty);
            mock.SomeProperty = arg3;
            Assert.Same(expected: arg3, mock.SomeProperty);
            context.AssertGet(f => f.SomeProperty);
            context.AssertSet(f => f.SomeProperty = arg3);
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.SomeProperty, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.SomeProperty = arg3, Invoked.Never));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.SomeProperty, Invoked.Exactly(2)));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.SomeProperty = arg3, Invoked.Exactly(2)));

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void AbstractClassWithEventSource()
        {
            var testScript = LoadAssembly<AAbstractClassWithEventSource>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.OnAbstractEvent += ExpectedEventHandler;
            mock.OnAbstractEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnAbstractEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnAbstractEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnAbstractEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnAbstractEvent -= UnexpectedEventHandler));

            mock.OnVirtualEvent += ExpectedEventHandler;
            mock.OnVirtualEvent -= ExpectedEventHandler;
            context.AssertAdd(f => f.OnVirtualEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnVirtualEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnVirtualEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnVirtualEvent -= UnexpectedEventHandler));

            Assert.Equal(expected: KExpected, testScript.DoRun());

            static void ExpectedEventHandler(object o, int a) { }
            static void UnexpectedEventHandler(object o, int a) { }
        }

        [Fact]
        public void AbstractClassWithEventSourceAndMultipleNamespaces()
        {
            var testScript = LoadAssembly<AAbstractClassWithEventSourceAndMultipleNamespaces>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.OnAbstractEvent += ExpectedEventHandler;
            mock.OnAbstractEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnAbstractEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnAbstractEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnAbstractEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnAbstractEvent -= UnexpectedEventHandler));

            mock.OnVirtualEvent += ExpectedEventHandler;
            mock.OnVirtualEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnVirtualEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnVirtualEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnVirtualEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnVirtualEvent -= UnexpectedEventHandler));

            Assert.Equal(expected: KExpected, testScript.DoRun());

            static void ExpectedEventHandler(object o, int a) { }
            static void UnexpectedEventHandler(object o, int a) { }
        }

        [Fact]
        public void GenericAbstractClassWithGenericEvent()
        {
            var testScript = LoadAssembly<AGenericAbstractClassWithGenericEvent<int>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.OnAbstractEvent += ExpectedEventHandler;
            mock.OnAbstractEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnAbstractEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnAbstractEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnAbstractEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnAbstractEvent -= UnexpectedEventHandler));

            mock.OnVirtualEvent += ExpectedEventHandler;
            mock.OnVirtualEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnVirtualEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnVirtualEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnVirtualEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnVirtualEvent -= UnexpectedEventHandler));

            Assert.Equal(expected: KExpected, testScript.DoRun());

            static void ExpectedEventHandler(object o, int a) { }
            static void UnexpectedEventHandler(object o, int a) { }
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

        [Fact]
        public void InheritAbstractClass()
        {
            var testScript = LoadAssembly<AInheritAbstractClass>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.Foo();
            mock.Bar();
            mock.Baz();
            mock.InvokeProtectedFoo();
            mock.InvokeProtectedBar();
            mock.InvokeProtectedBaz();

            context.Assert(f => f.Foo());
            context.Assert(f => f.Bar());
            context.Assert(f => f.Baz());

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void AbstractClassWithTaskMethod()
        {
            var testScript = LoadAssembly<AAbstractClassWithTaskMethod>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            var fooTask = mock.FooAsync();
            Assert.True(fooTask.IsCompleted);

            var barTask = mock.BarAsync();
            Assert.True(barTask.IsCompleted);
            Assert.Equal(default(int), barTask.Result);

            var bazTask = mock.BazAsync<AAbstractClassWithTaskMethod>();
            Assert.True(bazTask.IsCompleted);
            Assert.Equal(default(AAbstractClassWithTaskMethod), bazTask.Result);

            var fooVirtualTask = mock.VirtualFooAsync();
            Assert.True(fooVirtualTask.IsCompleted);

            var barVirtualTask = mock.VirtualBarAsync();
            Assert.True(barVirtualTask.IsCompleted);
            Assert.Equal(default(int), barVirtualTask.Result);

            var bazVirtualTask = mock.VirtualBazAsync<AAbstractClassWithTaskMethod>();
            Assert.True(bazVirtualTask.IsCompleted);
            Assert.Equal(default(AAbstractClassWithTaskMethod), bazVirtualTask.Result);

            var fooProtectedTask = mock.InvokeProtectedFooAsync();
            Assert.True(fooProtectedTask.IsCompleted);

            var barProtectedTask = mock.InvokeProtectedBarAsync();
            Assert.True(barProtectedTask.IsCompleted);
            Assert.Equal(default(int), barProtectedTask.Result);

            var bazProtectedTask = mock.InvokeProtectedBazAsync<AAbstractClassWithTaskMethod>();
            Assert.True(bazProtectedTask.IsCompleted);
            Assert.Equal(default(AAbstractClassWithTaskMethod), bazProtectedTask.Result);

            var fooProtectedVirtualTask = mock.InvokeProtectedVirtualFooAsync();
            Assert.True(fooProtectedVirtualTask.IsCompleted);

            var barProtectedVirtualTask = mock.InvokeProtectedVirtualBarAsync();
            Assert.True(barProtectedVirtualTask.IsCompleted);
            Assert.Equal(default(int), barProtectedVirtualTask.Result);

            var bazProtectedVirtualTask = mock.InvokeProtectedVirtualBazAsync<AAbstractClassWithTaskMethod>();
            Assert.True(bazProtectedVirtualTask.IsCompleted);
            Assert.Equal(default(AAbstractClassWithTaskMethod), bazProtectedVirtualTask.Result);

        }

        [Fact]
        public void CantProcessSealedClass()
        {
            var (diagnostics, success, assembly) = DoCompileResource();

            Assert.Contains(diagnostics, i => i.Id == "SPG005");
        }

        protected override string GetFullResourceName(string resourceName)
            => "AbstractClass." + resourceName + ".test.cs";
    }
}
