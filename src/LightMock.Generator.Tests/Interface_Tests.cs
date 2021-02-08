﻿using LightMock.Generator.Tests.Interface;
using LightMock.Generator.Tests.Interface.EventNamespace2;
using LightMock.Generator.Tests.Interface.Namespace1;
using LightMock.Generator.Tests.Interface.Namespace2;
using LightMock.Generator.Tests.TestAbstractions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class Interface_Tests : GeneratorTestsBase
    {
        const int KExpected = 42;

        public Interface_Tests(ITestOutputHelper testOutputHelper)
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
        public void InterfaceWithBasicProperty()
        {
            var testScript = LoadAssembly<IInterfaceWithBasicProperty>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.OnlyGet));
            Assert.Throws<InvalidOperationException>(() => context.AssertSet(f => f.GetAndSet = 5678));
            Assert.Throws<InvalidOperationException>(() => context.AssertGet(f => f.GetAndSet));

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, mock.OnlyGet);
            context.AssertGet(f => f.OnlyGet);

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
            mock.GenericWithClassConstraint(p);
            context.Assert(f => f.GenericWithClassConstraint(p));

            mock.GenericWithStructConstraint<int>(1234);
            context.Assert(f => f.GenericWithStructConstraint<int>(1234));

            mock.GenericWithConstraint(1234);
            context.Assert(f => f.GenericWithConstraint(1234));

            mock.GenericWithManyConstraints<object, int, long>(p, 123, 456);
            context.Assert(f => f.GenericWithManyConstraints<object, int, long>(p, 123, 456));
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
        }

        [Fact]
        public void InterfaceWithEventSource()
        {
            var testScript = LoadAssembly<IInterfaceWithEventSource>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.OnEvent += ExpectedEventHandler;
            mock.OnEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnEvent -= UnexpectedEventHandler));

            Assert.Equal(expected: KExpected, testScript.DoRun());

            static void ExpectedEventHandler(object o, int a) { }
            static void UnexpectedEventHandler(object o, int a) { }
        }

        [Fact]
        public void InterfaceWithEventSourceAndMultipleNamespaces()
        {
            var testScript = LoadAssembly<IInterfaceWithEventSourceAndMultipleNamespaces>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.OnEvent += ExpectedEventHandler;
            mock.OnEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnEvent -= UnexpectedEventHandler));

            Assert.Equal(expected: KExpected, testScript.DoRun());

            static void ExpectedEventHandler(object o, int a) { }
            static void UnexpectedEventHandler(object o, int a) { }
        }

        [Fact]
        public void GenericInterfaceWithGenericEvent()
        {
            var testScript = LoadAssembly<IGenericInterfaceWithGenericEvent<int>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.OnEvent += ExpectedEventHandler;
            mock.OnEvent -= ExpectedEventHandler;

            context.AssertAdd(f => f.OnEvent += ExpectedEventHandler);
            context.AssertRemove(f => f.OnEvent -= ExpectedEventHandler);
            Assert.Throws<InvalidOperationException>(() => context.AssertAdd(f => f.OnEvent += UnexpectedEventHandler));
            Assert.Throws<InvalidOperationException>(() => context.AssertRemove(f => f.OnEvent -= UnexpectedEventHandler));

            Assert.Equal(expected: KExpected, testScript.DoRun());

            static void ExpectedEventHandler(object o, int a) { }
            static void UnexpectedEventHandler(object o, int a) { }
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
        public void InherittedInterface()
        {
            var testScript = LoadAssembly<IInherittedInterface>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.Foo();
            mock.Bar();
            mock.Baz();
            mock.Quux();

            context.Assert(f => f.Foo());
            context.Assert(f => f.Bar());
            context.Assert(f => f.Baz());
            context.Assert(f => f.Quux());

            Assert.Equal(expected: KExpected, testScript.DoRun());
        }

        [Fact]
        public void GenericInterfaceWithVariance()
        {
            var testScript = LoadAssembly<IGenericInterfaceWithVariance<IFoo, IBar>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.Foo();

            context.Assert(f => f.Foo());

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void InterfaceWithTaskMethod()
        {
            var testScript = LoadAssembly<IInterfaceWithTaskMethod>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            var fooTask = mock.FooAsync();
            Assert.True(fooTask.IsCompleted);

            var barTask = mock.BarAsync();
            Assert.True(barTask.IsCompleted);
            Assert.Equal(default(int), barTask.Result);

            var bazTask = mock.BazAsync<IInterfaceWithTaskMethod>();
            Assert.True(bazTask.IsCompleted);
            Assert.Equal(default(IInterfaceWithTaskMethod), bazTask.Result);
        }

        protected override string GetFullResourceName(string resourceName)
            => "Interface." + resourceName + ".test.cs";
    }
}