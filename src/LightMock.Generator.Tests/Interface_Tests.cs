using LightMock.Generator.Tests.Interface;
using LightMock.Generator.Tests.Interface.EventNamespace2;
using LightMock.Generator.Tests.Interface.Namespace1;
using LightMock.Generator.Tests.Interface.Namespace2;
using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Text;
using System.Threading.Tasks;
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

            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 5678));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet));

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, mock.OnlyGet);
            context.AssertGet(f => f.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 5678;
            Assert.Equal(5678, mock.GetAndSet);
            context.AssertSet_When(f => f.GetAndSet = 5678);
            context.AssertGet(f => f.GetAndSet);

            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 1234));

            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 5678, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Never));

            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Exactly(2)));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 5678, Invoked.Exactly(2)));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Exactly(2)));
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


            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet));
            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, mock.OnlyGet);
            context.AssertGet(f => f.OnlyGet);
            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Exactly(2)));

            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 3456));
            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 3456;
            Assert.Equal(3456, mock.GetAndSet);
            context.AssertGet(f => f.GetAndSet);
            context.AssertSet_When(f => f.GetAndSet = 3456);
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 1234));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 3456, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Exactly(2)));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 3456, Invoked.Exactly(2)));
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

            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet));
            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, mock.OnlyGet);
            context.AssertGet(f => f.OnlyGet);
            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet, Invoked.Exactly(2)));

            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 3456));
            context.ArrangeProperty(f => f.GetAndSet);
            mock.GetAndSet = 3456;
            Assert.Equal(3456, mock.GetAndSet);
            context.AssertGet(f => f.GetAndSet);
            context.AssertSet_When(f => f.GetAndSet = 3456);
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 1234));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 3456, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet, Invoked.Exactly(2)));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 3456, Invoked.Exactly(2)));

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
            Assert.Throws<MockException>(() => context.AssertGet(f => f.SomeProperty));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.SomeProperty = arg3));
            context.ArrangeProperty(f => f.SomeProperty);
            mock.SomeProperty = arg3;
            Assert.Same(expected: arg3, mock.SomeProperty);
            context.AssertGet(f => f.SomeProperty);
            context.AssertSet_When(f => f.SomeProperty = arg3);
            Assert.Throws<MockException>(() => context.AssertGet(f => f.SomeProperty, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.SomeProperty = arg3, Invoked.Never));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.SomeProperty, Invoked.Exactly(2)));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.SomeProperty = arg3, Invoked.Exactly(2)));
        }

        [Fact]
        public void InterfaceWithEventSource()
        {
            var testScript = LoadAssembly<IInterfaceWithEventSource>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Throws<MockException>(() => context.AssertAdd_WhenAny(f => f.OnEvent += null));
            Assert.Throws<MockException>(() => context.AssertRemove_WhenAny(f => f.OnEvent -= null));

            mock.OnEvent += ExpectedEventHandler;
            mock.OnEvent -= ExpectedEventHandler;

            context.AssertAdd_When(f => f.OnEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnEvent -= ExpectedEventHandler);
            context.AssertAdd_WhenAny(f => f.OnEvent += null);
            context.AssertRemove_WhenAny(f => f.OnEvent -= null);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnEvent -= UnexpectedEventHandler));

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

            context.AssertAdd_When(f => f.OnEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnEvent -= ExpectedEventHandler);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnEvent -= UnexpectedEventHandler));

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

            context.AssertAdd_When(f => f.OnEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnEvent -= ExpectedEventHandler);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnEvent -= UnexpectedEventHandler));

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
            mock.GetResult();
            ((IFoo)mock).GetResult();
            ((IBar)mock).GetResult();
            ((IBaz)mock).GetResult();
            _ = mock.Result;
            _ = ((IFoo)mock).Result;
            _ = ((IBar)mock).Result;
            _ = ((IBaz)mock).Result;

            context.Assert(f => f.Foo());
            context.Assert(f => f.Bar());
            context.Assert(f => f.Baz());
            context.Assert(f => f.Quux());
            context.Assert(f => f.GetResult());
            context.Assert(f => ((IFoo)f).GetResult());
            context.Assert(f => ((IBar)f).GetResult());
            context.Assert(f => ((IBaz)f).GetResult());
            context.AssertGet(f => f.Result);
            context.AssertGet(f => ((IFoo)f).Result);
            context.AssertGet(f => ((IBar)f).Result);
            context.AssertGet(f => ((IBaz)f).Result);

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

        [Fact]
        public async Task InterfaceWithEnumerableResult()
        {
            var testScript = LoadAssembly<IInterfaceWithEnumerableResult>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(mock.CollectionProperty);
            Assert.NotNull(mock.GetCollection());
            Assert.NotNull(mock.GetGenericCollection<int>());
            Assert.NotNull(await mock.GetCollectionAsync());
            Assert.NotNull(await mock.GetGenericCollectionAsync<int>());

            Assert.Empty(mock.CollectionProperty);
            Assert.Empty(mock.GetCollection());
            Assert.Empty(mock.GetGenericCollection<int>());
            Assert.Empty(await mock.GetCollectionAsync());
            Assert.Empty(await mock.GetGenericCollectionAsync<int>());
        }

        [Fact]
        public void InterfaceThrowsExceptionOnRefStruct()
        {
            var testScript = LoadAssembly<IInterfaceThrowsExceptionOnRefStruct>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            var span = new Span<int>(new int[1024]);
            InvalidProgramException? exception = null;
            try
            {
                mock.Foo(span);
            }
            catch (InvalidProgramException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.Equal(ExceptionMessages.OnRefStructMethod, exception?.Message);

            exception = null;
            try
            {
                mock.Bar(span);
            }
            catch (InvalidProgramException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.Equal(ExceptionMessages.OnRefStructMethod, exception?.Message);
        }

        [Fact]
        public void InterfaceNotGenerated()
        {
            var testScript = LoadAssembly<IInterfaceNotGenerated>();
            
            Assert.Throws<MockNotGeneratedException>(() => testScript.Context);
            Assert.Throws<MockNotGeneratedException>(() => testScript.MockObject);
        }

        [Fact]
        public void NestedInterface()
        {
            var testScript = LoadAssembly<INestedInterface.ITest>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.Foo();
            context.Assert(f => f.Foo());
        }

        [Fact]
        public void NestedGenericInterface()
        {
            var testScript = LoadAssembly<INestedGenericInterface<int>.IContainingInterface<long>.ITest<IBar>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.Foo(2345)).Returns(1234);
            Assert.Equal(1234, mock.Foo(2345));
            context.Assert(f => f.Foo(2345));

            context.Arrange(f => f.Bar<int>(6789)).Returns(5678);
            Assert.Equal(5678, mock.Bar<int>(6789));
            context.Assert(f => f.Bar<int>(6789));

            context.Arrange(f => f.Baz(null)).Returns(9012);
            Assert.Equal(9012, mock.Baz(null));
            context.Assert(f => f.Baz(null));

        }

        [Fact]
        public void ArrangeSetter()
        {
            var testScript = LoadAssembly<IArrangeSetter>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Equal(KExpected, testScript.DoRun());
            mock.GetAndSet = "4567";
            mock.Set = "1234";
            Assert.Throws<ValidProgramException>(() => mock.GetAndSet = "1234");
            Assert.Throws<ValidProgramException>(() => mock.Set = "4567");
        }

        [Fact]
        public void AssertSet()
        {
            var testScript = LoadAssembly<IAssertSet>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Throws<MockException>(() => testScript.DoRun());

            mock.GetAndSet = "jskldjalsdjljl";
            mock.SetOnly = "hello world";

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void AssertSet_WhenAny()
        {
            var testScript = LoadAssembly<IAssertSet_WhenAny>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.GetAndSet = Guid.NewGuid().ToString();
            mock.SetOnly = Guid.NewGuid().ToString();

            context.AssertSet_WhenAny(f => f.GetAndSet = "");
            context.AssertSet_WhenAny(f => f.GetAndSet = "", Invoked.Once);
            context.AssertSet_WhenAny(f => f.SetOnly = "");
            context.AssertSet_WhenAny(f => f.SetOnly = "", Invoked.Once);

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void ArrangeSetter_WhenAny()
        {
            var expected = Guid.NewGuid().ToString();
            var testScript = LoadAssembly<IArrangeSetter_WhenAny>();
            var context = testScript.Context;
            var mock = testScript.MockObject;
            string actual = "";

            context.ArrangeSetter_WhenAny(f => f.SetOnly = "").Callback<string>(s => actual = s);

            mock.SetOnly = expected;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArrangeSetter_When()
        {
            int on1234 = 0;
            int on5678 = 0;
            var testScript = LoadAssembly<IArrangeSetter_When>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.ArrangeSetter_When(f => f.SetOnly = "1234").Callback<string>(s => on1234++);
            context.ArrangeSetter_When(f => f.SetOnly = "5678").Callback<string>(s => on5678++);

            mock.SetOnly = "1234";

            Assert.Equal(1, on1234);
            Assert.Equal(0, on5678);
        }

        [Fact]
        public void InheritSpecialized()
        {
            var expected = new SpecializationTag();
            var testScript = LoadAssembly<IInheritSpecialized>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.Function()).Returns(expected);
            Assert.Same(expected, mock.Function());

            mock.Action(expected);
            context.Assert(f => f.Action(expected));
        }

        [Fact]
        public void Indexer()
        {
            string expectedValue = Guid.NewGuid().ToString();
            int expectedIndex = new Random().Next();
            var testScript = LoadAssembly<IIndexer<string>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f[The<int>.IsAnyValue]).Returns(expectedValue);
            Assert.Equal(expectedValue, mock[expectedIndex]);

            (int index, string value) invokedWith = default;
            context.ArrangeSetter_WhenAny(f => f[0] = "").Callback<int, string>((i, s) => invokedWith = (i, s));
            mock[expectedIndex] = expectedValue;
            Assert.Equal(expectedValue, invokedWith.value);
            Assert.Equal(expectedIndex, invokedWith.index);
        }

        [Fact]
        public void AssertNoOtherCalls()
        {
            var testScript = LoadAssembly<IAssertNoOtherCalls>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.GetAndSet = nameof(mock.GetAndSet);
            mock.SetOnly = nameof(mock.SetOnly);
            mock["123"] = "indexer_set";

            _ = mock.GetAndSet;
            _ = mock.GetOnly;

            _ = mock.Function(nameof(mock.Function));
            mock.Method(nameof(mock.Method));

            _ = mock["456"];

            mock.EventHandler += SomeEventHandler;
            mock.EventHandler -= SomeEventHandler;

            context.AssertSet_When(f => f.GetAndSet = nameof(mock.GetAndSet));
            context.AssertSet_When(f => f.SetOnly = nameof(mock.SetOnly));

            context.AssertGet(f => f.GetAndSet);
            context.AssertGet(f => f.GetOnly);

            context.Assert(f => f.Function(nameof(mock.Function)));
            context.Assert(f => f.Method(nameof(mock.Method)));

            context.AssertGet(f => f["456"]);
            context.AssertSet_When(f => f["123"] = "indexer_set");

            context.AssertAdd_When(f => f.EventHandler += SomeEventHandler);
            context.AssertRemove_When(f => f.EventHandler -= SomeEventHandler);

            Assert.Equal(KExpected, testScript.DoRun());

            context.AssertNoOtherCalls();

            void SomeEventHandler(object? sender, EventArgs args) { }
        }

        [Fact]
        public void AssertNoOtherCalls_Throws()
        {
            EventHandler eh = SomeEventHandler;
            var expectedMessage = new StringBuilder()
                .AppendLine("Detected unverified invocations: ")
                .AppendLine("System.String LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.Function(System.String a = \"Function\")")
                .AppendLine("System.Void LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.Method(System.String a = \"Method\")")
                .AppendLine("System.Void LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.GetAndSet = \"GetAndSet\"")
                .AppendLine("System.Void LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.SetOnly = \"SetOnly\"")
                .AppendLine("System.Void LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws[string \"123\"] = \"indexer_set\"")
                .AppendLine("System.String LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.GetAndSet")
                .AppendLine("System.String LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.GetOnly")
                .AppendLine("System.String LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws[string \"456\"]")
                .Append("System.Void LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.EventHandler += ")
                .AppendLine(eh.Method.ToString())
                .Append("System.Void LightMock.Generator.Tests.Interface.IAssertNoOtherCalls_Throws.EventHandler -= ")
                .AppendLine(eh.Method.ToString())
                .ToString();

            var testScript = LoadAssembly<IAssertNoOtherCalls_Throws>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.GetAndSet = nameof(mock.GetAndSet);
            mock.SetOnly = nameof(mock.SetOnly);
            mock["123"] = "indexer_set";

            _ = mock.GetAndSet;
            _ = mock.GetOnly;

            _ = mock.Function(nameof(mock.Function));
            mock.Method(nameof(mock.Method));

            _ = mock["456"];

            mock.EventHandler += SomeEventHandler;
            mock.EventHandler -= SomeEventHandler;

            var ex = Assert.Throws<MockException>(() => context.AssertNoOtherCalls());
            Assert.Equal(expectedMessage, ex.Message);

            void SomeEventHandler(object? sender, EventArgs args) { }
        }

        [Fact]
        public void ArrangeAddRemove_When()
        {
            var testScript = LoadAssembly<IArrangeAddRemove_When>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.ArrangeAdd_When(f => f.EventHandler += SomeEventHandler).Throws<ValidProgramException>();
            context.ArrangeRemove_When(f => f.EventHandler -= SomeEventHandler).Throws<ValidProgramException>();

            Assert.Throws<ValidProgramException>(() =>  mock.EventHandler += SomeEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.EventHandler -= SomeEventHandler);

            mock.EventHandler += AnotherEventHandler;
            mock.EventHandler -= AnotherEventHandler;

            void SomeEventHandler(object? o, EventArgs a) { }
            void AnotherEventHandler(object? o, EventArgs a) { }
        }

        [Fact]
        public void ArrangeAddRemove_WhenAny()
        {
            var testScript = LoadAssembly<IArrangeAddRemove_WhenAny>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.ArrangeAdd_WhenAny(f => f.EventHandler += null).Throws<ValidProgramException>();
            context.ArrangeRemove_WhenAny(f => f.EventHandler -= null).Throws<ValidProgramException>();

            Assert.Throws<ValidProgramException>(() => mock.EventHandler += SomeEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.EventHandler -= SomeEventHandler);

            Assert.Throws<ValidProgramException>(() => mock.EventHandler += AnotherEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.EventHandler -= AnotherEventHandler);

            void SomeEventHandler(object? o, EventArgs a) { }
            void AnotherEventHandler(object? o, EventArgs a) { }
        }

        [Fact]
        public void ReservedSymbols()
        {
            var testScript = LoadAssembly<IReservedSymbols>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(context);
            Assert.NotNull(mock);
        }

        [Fact]
        // Issue #52
        public void MethodWithOutParameter()
        {
            const int EXPECTED_OUT = 321;
            const int EXPECTED_RESULT = 123;
            var testScript = LoadAssembly<IMethodWithOutParameter>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(context);
            Assert.NotNull(mock);

            context.Arrange(_ => _.Foo(out The<int>.Reference.IsAny.Value))
                .Returns(EXPECTED_RESULT)
                .Callback(FooCallback);

            int bar;
            Assert.Equal(EXPECTED_RESULT, mock.Foo(out bar));
            Assert.Equal(EXPECTED_OUT, bar);

            context.Assert(_ => _.Foo(out The<int>.Reference.IsAny.Value), Invoked.Once);

            static void FooCallback(out int bar) => bar = EXPECTED_OUT;
        }

        [Fact]
        // Issue #52
        public void MethodWithRefParameter()
        {
            const string EXPECTED_OUT = nameof(EXPECTED_OUT);
            const string EXPECTED_RESULT = nameof(EXPECTED_RESULT);
            var testScript = LoadAssembly<IMethodWithRefParameter>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(context);
            Assert.NotNull(mock);

            context.Arrange(_ => _.Foo(ref The<string>.Reference.IsAny.Value))
                .Returns(EXPECTED_RESULT)
                .Callback(FooCallback);

            string bar = "";
            Assert.Equal(EXPECTED_RESULT, mock.Foo(ref bar));
            Assert.Equal(EXPECTED_OUT, bar);

            context.Assert(_ => _.Foo(ref The<string>.Reference.IsAny.Value), Invoked.Once);

            static void FooCallback(out string bar) => bar = EXPECTED_OUT;
        }

        [Fact]
        // Issue #54
        public void MethodWithInParameter()
        {
            const string EXPECTED_IN = nameof(EXPECTED_IN);
            const string EXPECTED_RESULT = nameof(EXPECTED_RESULT);
            var testScript = LoadAssembly<IMethodWithInParameter>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(context);
            Assert.NotNull(mock);

            context.Arrange(_ => _.Foo(in The<string>.Reference.IsAny.Value))
                .Returns(EXPECTED_RESULT);

            string bar = EXPECTED_IN;
            Assert.Equal(EXPECTED_RESULT, mock.Foo(in bar));

            context.Assert(_ => _.Foo(in The<string>.Reference.Is(s => s == EXPECTED_IN).Value), Invoked.Once);
        }

        protected override string GetFullResourceName(string resourceName)
            => "Interface." + resourceName + ".test.cs";
    }
}
