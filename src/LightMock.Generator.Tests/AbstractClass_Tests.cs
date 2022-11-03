using LightMock.Generator.Tests.AbstractClass;
using LightMock.Generator.Tests.AbstractClass.EventNamespace2;
using LightMock.Generator.Tests.AbstractClass.Namespace1;
using LightMock.Generator.Tests.AbstractClass.Namespace2;
using LightMock.Generator.Tests.TestAbstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
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

            Assert.Throws<MockException>(() => context.AssertGet(f => f.OnlyGet));
            Assert.Throws<MockException>(() => context.AssertSet_When(f => f.GetAndSet = 5678));
            Assert.Throws<MockException>(() => context.AssertGet(f => f.GetAndSet));

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, mock.OnlyGet);

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

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void AbstractClassWithEventSource()
        {
            var testScript = LoadAssembly<AAbstractClassWithEventSource>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Throws<MockException>(() => context.AssertAdd_WhenAny(f => f.OnAbstractEvent += null));
            Assert.Throws<MockException>(() => context.AssertRemove_WhenAny(f => f.OnAbstractEvent -= null));

            mock.OnAbstractEvent += ExpectedEventHandler;
            mock.OnAbstractEvent -= ExpectedEventHandler;

            context.AssertAdd_When(f => f.OnAbstractEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnAbstractEvent -= ExpectedEventHandler);
            context.AssertAdd_WhenAny(f => f.OnAbstractEvent += null);
            context.AssertAdd_WhenAny(f => f.OnAbstractEvent -= null);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnAbstractEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnAbstractEvent -= UnexpectedEventHandler));

            mock.OnVirtualEvent += ExpectedEventHandler;
            mock.OnVirtualEvent -= ExpectedEventHandler;
            context.AssertAdd_When(f => f.OnVirtualEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnVirtualEvent -= ExpectedEventHandler);
            context.AssertAdd_WhenAny(f => f.OnVirtualEvent += null);
            context.AssertRemove_WhenAny(f => f.OnVirtualEvent -= null);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnVirtualEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnVirtualEvent -= UnexpectedEventHandler));

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

            context.AssertAdd_When(f => f.OnAbstractEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnAbstractEvent -= ExpectedEventHandler);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnAbstractEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnAbstractEvent -= UnexpectedEventHandler));

            mock.OnVirtualEvent += ExpectedEventHandler;
            mock.OnVirtualEvent -= ExpectedEventHandler;

            context.AssertAdd_When(f => f.OnVirtualEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnVirtualEvent -= ExpectedEventHandler);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnVirtualEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnVirtualEvent -= UnexpectedEventHandler));

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

            context.AssertAdd_When(f => f.OnAbstractEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnAbstractEvent -= ExpectedEventHandler);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnAbstractEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnAbstractEvent -= UnexpectedEventHandler));

            mock.OnVirtualEvent += ExpectedEventHandler;
            mock.OnVirtualEvent -= ExpectedEventHandler;

            context.AssertAdd_When(f => f.OnVirtualEvent += ExpectedEventHandler);
            context.AssertRemove_When(f => f.OnVirtualEvent -= ExpectedEventHandler);
            Assert.Throws<MockException>(() => context.AssertAdd_When(f => f.OnVirtualEvent += UnexpectedEventHandler));
            Assert.Throws<MockException>(() => context.AssertRemove_When(f => f.OnVirtualEvent -= UnexpectedEventHandler));

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

        [Fact]
        public async Task AbstractClassWithEnumerableResult()
        {
            var testScript = LoadAssembly<AAbstractClassWithEnumerableResult>();
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
        public void AbstractClassThrowsExceptionOnRefStruct()
        {
            var testScript = LoadAssembly<AAbstractClassThrowsExceptionOnRefStruct>();
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
        public void AbstractClassNotGenerated()
        {
            var testScript = LoadAssembly<AAbstractClassNotGenerated>();

            Assert.Throws<MockNotGeneratedException>(() => testScript.Context);
            Assert.Throws<MockNotGeneratedException>(() => testScript.MockObject);
        }

        [Fact]
        public void NestedClass()
        {
            var testScript = LoadAssembly<XNestedClass.ATest>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.Foo();
            context.Assert(f => f.Foo());
        }

        [Fact]
        public void ObsoleteSupport()
        {
            var fn = GetFullResourceName(nameof(ObsoleteSupport));
            var source = Utils.LoadResource(fn);
            var compilation = CreateCompilation(source, fn);
            var driver = CreateGenerationDriver(compilation);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var ms = new MemoryStream();
            var result = updatedCompilation.Emit(ms);
            Assert.DoesNotContain(result.Diagnostics, f => f.Id == "CS0672");
        }

        [Fact]
        public void NestedGenericClass()
        {
            var testScript = LoadAssembly<ANestedGenericClass<int>.AContainingClass<long>.ATest<AFoo>>();
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
        public void DontOverrideSupport()
        {
            var testScript = LoadAssembly<ADontOverrideSupport>(
                Params(nameof(DontOverrideSupport),
                nameof(DontOverrideSupport) + ".AssemblyInfo"));
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.Baz();
            mock.Quux();
            mock.Quuux();
            mock.Quuuux();
            mock.Quuuuux();

            context.Assert(f => f.Baz());
            context.Assert(f => f.Quux());
            context.Assert(f => f.Quuux());
            context.Assert(f => f.Quuuux());
            context.Assert(f => f.Quuuuux());

            Assert.Throws<InvalidProgramException>(() => mock.Foo());
            Assert.Throws<InvalidProgramException>(() => mock.Bar());
        }

        [Fact]
        public void ArrangeSetter()
        {
            var testScript = LoadAssembly<AArrangeSetter>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Equal(KExpected, testScript.DoRun());
            mock.GetAndSet = "4567";
            mock.SetOnly = "1234";
            mock.InvokeProtectedGetAndSet = "2345";
            mock.InvokeProtectedSetOnly = "8901";
            Assert.Equal(nameof(mock.GetAndSet), Assert.Throws<ValidProgramException>(() => mock.GetAndSet = "1234").Message);
            Assert.Equal(nameof(mock.SetOnly), Assert.Throws<ValidProgramException>(() => mock.SetOnly = "4567").Message);
            Assert.Equal("ProtectedGetAndSet", Assert.Throws<ValidProgramException>(() => mock.InvokeProtectedGetAndSet = "8901").Message);
            Assert.Equal("ProtectedSetOnly", Assert.Throws<ValidProgramException>(() => mock.InvokeProtectedSetOnly = "2345").Message);
        }

        [Fact]
        public void AssertSet()
        {
            var testScript = LoadAssembly<AAssertSet>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.Throws<MockException>(() => testScript.DoRun());

            mock.GetAndSet = "jskldjalsdjljl";
            mock.SetOnly = "hello world";
            mock.InvokeProtectedGetAndSet = "jskldjalsdjljl";
            mock.InvokeProtectedSetOnly = "hello world";

            Assert.Equal(KExpected, testScript.DoRun());
        }

        [Fact]
        public void AssertSet_WhenAny()
        {
            var testScript = LoadAssembly<AAssertSet_WhenAny>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.GetAndSet = Guid.NewGuid().ToString();
            mock.SetOnly = Guid.NewGuid().ToString();
            mock.InvokeProtectedGetAndSet = Guid.NewGuid().ToString();
            mock.InvokeProtectedSetOnly = Guid.NewGuid().ToString();

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
            var testScript = LoadAssembly<AArrangeSetter_WhenAny>();
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
            var testScript = LoadAssembly<AArrangeSetter_When>();
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
            var testScript = LoadAssembly<AInheritSpecialized>();
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
            var testScript = LoadAssembly<AIndexer<string>>();
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
            var testScript = LoadAssembly<AAssertNoOtherCalls>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.GetAndSet = nameof(mock.GetAndSet);
            mock.SetOnly = nameof(mock.SetOnly);

            _ = mock.GetAndSet;
            _ = mock.GetOnly;

            _ = mock.Function(nameof(mock.Function));
            mock.Method(nameof(mock.Method));

            mock.InvokeProtectedGetAndSet = nameof(mock.GetAndSet);
            mock.InvokeProtectedSetOnly = nameof(mock.SetOnly);
            mock.InvokeIndexerSet("123", "indexer_set");

            _ = mock.InvokeProtectedGetAndSet;
            _ = mock.InvokeProtectedGetOnly;
            _ = mock.InvokeIndexerGet("456");

            _ = mock.InvokeProtectedFunction(nameof(mock.Function));
            mock.InvokeProtectedMethod(nameof(mock.Method));

            context.AssertSet_When(f => f.GetAndSet = nameof(mock.GetAndSet));
            context.AssertSet_When(f => f.SetOnly = nameof(mock.SetOnly));

            context.AssertGet(f => f.GetAndSet);
            context.AssertGet(f => f.GetOnly);

            context.Assert(f => f.Function(nameof(mock.Function)));
            context.Assert(f => f.Method(nameof(mock.Method)));

            Assert.Equal(KExpected, testScript.DoRun());

            context.AssertNoOtherCalls();
        }

        [Fact]
        public void AssertNoOtherCalls_Throws()
        {
            var expectedMessage = new StringBuilder()
                .AppendLine("Detected unverified invocations: ")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.Function(System.String a = \"Function\")")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.Method(System.String a = \"Method\")")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.ProtectedFunction(System.String a = \"Function\")")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.ProtectedMethod(System.String a = \"Method\")")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.GetAndSet = \"GetAndSet\"")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.SetOnly = \"SetOnly\"")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.GetAndSet")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.GetOnly")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.ProtectedGetAndSet = \"GetAndSet\"")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.ProtectedSetOnly = \"SetOnly\"")
                .AppendLine("System.Void LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws[string \"123\"] = \"indexer_set\"")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.ProtectedGetAndSet")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws.ProtectedGetOnly")
                .AppendLine("System.String LightMock.Generator.Tests.AbstractClass.AAssertNoOtherCalls_Throws[string \"456\"]")
                .ToString();

            var testScript = LoadAssembly<AAssertNoOtherCalls_Throws>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock.GetAndSet = nameof(mock.GetAndSet);
            mock.SetOnly = nameof(mock.SetOnly);

            _ = mock.GetAndSet;
            _ = mock.GetOnly;

            _ = mock.Function(nameof(mock.Function));
            mock.Method(nameof(mock.Method));

            mock.InvokeProtectedGetAndSet = nameof(mock.GetAndSet);
            mock.InvokeProtectedSetOnly = nameof(mock.SetOnly);
            mock.InvokeIndexerSet("123", "indexer_set");

            _ = mock.InvokeProtectedGetAndSet;
            _ = mock.InvokeProtectedGetOnly;
            _ = mock.InvokeIndexerGet("456");

            _ = mock.InvokeProtectedFunction(nameof(mock.Function));
            mock.InvokeProtectedMethod(nameof(mock.Method));

            var ex = Assert.Throws<MockException>(() => context.AssertNoOtherCalls());
            Assert.Equal(expectedMessage, ex.Message);
        }

        [Fact]
        public void ArrangeAddRemove_When()
        {
            var testScript = LoadAssembly<AArrangeAddRemove_When>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.ArrangeAdd_When(f => f.EventHandler += SomeEventHandler).Throws<ValidProgramException>();
            context.ArrangeRemove_When(f => f.EventHandler -= SomeEventHandler).Throws<ValidProgramException>();

            Assert.Throws<ValidProgramException>(() => mock.EventHandler += SomeEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.EventHandler -= SomeEventHandler);

            mock.EventHandler += AnotherEventHandler;
            mock.EventHandler -= AnotherEventHandler;

            Assert.Equal(KExpected, testScript.DoRun());

            void SomeEventHandler(object? o, EventArgs a) { }
            void AnotherEventHandler(object? o, EventArgs a) { }
        }

        [Fact]
        public void ArrangeAddRemove_WhenAny()
        {
            var testScript = LoadAssembly<AArrangeAddRemove_WhenAny>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.ArrangeAdd_WhenAny(f => f.EventHandler += null).Throws<ValidProgramException>();
            context.ArrangeRemove_WhenAny(f => f.EventHandler -= null).Throws<ValidProgramException>();

            Assert.Throws<ValidProgramException>(() => mock.EventHandler += SomeEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.EventHandler -= SomeEventHandler);

            Assert.Throws<ValidProgramException>(() => mock.EventHandler += AnotherEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.EventHandler -= AnotherEventHandler);

            Assert.Equal(KExpected, testScript.DoRun());

            Assert.Throws<ValidProgramException>(() => mock.InvokeProtectedEventHandler += SomeEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.InvokeProtectedEventHandler -= SomeEventHandler);

            Assert.Throws<ValidProgramException>(() => mock.InvokeProtectedEventHandler += AnotherEventHandler);
            Assert.Throws<ValidProgramException>(() => mock.InvokeProtectedEventHandler -= AnotherEventHandler);

            void SomeEventHandler(object? o, EventArgs a) { }
            void AnotherEventHandler(object? o, EventArgs a) { }
        }

        [Fact]
        public void ReservedSymbols()
        {
            var testScript = LoadAssembly<AReservedSymbols>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(context);
            Assert.NotNull(mock);
        }

        [Fact]
        public void GenerateForInternalClass()
        {
            var (diagnostics, success, assembly) = DoCompileResource();

            Assert.True(success);
            Assert.Empty(diagnostics);

            const string testClassName = nameof(GenerateForInternalClass);
            var alc = new AssemblyLoadContext(testClassName);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var testClassType = loadedAssembly.DefinedTypes.Where(t => t.Name == testClassName).First();
            var testClass = Activator.CreateInstance(testClassType) ?? throw new InvalidOperationException("can't create test class");
            Assert.NotNull(testClass);
        }

        [Fact]
        // Issue #52
        public void MethodWithOutParameter()
        {
            var testScript = LoadAssembly<AMethodWithOutParameter>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            Assert.NotNull(context);
            Assert.NotNull(mock);
            var result = mock.Foo(out var bar);
            Assert.Equal(0, bar);
            Assert.Equal(0, result);
        }

        protected override string GetFullResourceName(string resourceName)
            => "AbstractClass." + resourceName + ".test.cs";
    }
}
