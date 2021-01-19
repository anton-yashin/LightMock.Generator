# LightMock.Generator (Beta)

Source generator that generates mocks by provided interfaces and abstract classes. 
You should be familiar with [LigthMock](https://github.com/seesharper/LightMock) because this project uses it underhood.
[Avaialble on GitHub](https://github.com/anton-yashin/LightMock.Generator)
[Available on nuget](https://www.nuget.org/packages/LightMock.Generator/).

## How to use
* Use Mock<T> where T is you abstract class or interface to batch create MockContext<T> and mock object.
* Create mock partial class that implement an interface or abstract class and decorate it with attribute [GenerateMock]

## Example with Mock<T> and interface

```csharp
using System;
using LightMock;
using LightMock.Generator;
using Xunit;

namespace Playground
{
    public interface IFoo
    {
        void Foo(int baz);
        int Bar();
    }

    public class SomeTests
    {
        [Fact]
        public void Test()
        {
            var mock = new Mock<IFoo>();
            var o = mock.Object; // use Mock<T>.Object property to get mock object

            o.Foo(123);
            mock.Assert(f => f.Foo(123)); // Mock<T> inherit MockContext<T>. Use it to assert or arrange context.

            const int expected = 123;
            mock.Arrange(f => f.Bar()).Returns(expected); // Mock<T> inherit MockContext<T>. Use it to assert or arrange context.

            Assert.Equal(expected, o.Bar());
        }
    }
}

```

## Example with Mock<T> and abstract class

```csharp
using System;
using LightMock.Generator;
using Xunit;

namespace Playground
{
    public abstract class AFoo
    {
        public AFoo(int p1, int p2)
        { }

        public abstract void Foo(int p);
        public abstract int Bar();

        protected abstract void Baz(int p);
        protected abstract int Quux();

        public void InvokeBaz(int p) => Baz(p);
        public int InvokeQuux() => Quux();
    }


    public class SomeTests
    {
        [Fact]
        public void Test()
        {
            const int expected = 123;
            // To invoke a constructor of abstract class place parameters in Mock<T> constructor
            var mock = new Mock<AFoo>(12, 45);
            // To arrange or assert protected members call Protected() extension function.
            // It and corresponding interface will be generated only for abstract classes
            mock.Protected().Arrange(f => f.Quux()).Returns(expected);

            Assert.Equal(expected, mock.Object.InvokeQuux());
            mock.Protected().Assert(f => f.Quux());

            // To arrange or assert public members use Mock<T> functions
            mock.Arrange(f => f.Bar()).Returns(expected);
            Assert.Equal(expected, mock.Object.Bar());
            mock.Assert(f => f.Bar());
        }
    }
}

```

## Example with [GenerateMock] attribute

```csharp
using System;
using LightMock;
using LightMock.Generator;
using Xunit;

namespace Playground
{
    public interface IFoo
    {
        void Foo(int p);
        int Bar();
    }


    public abstract class AFoo
    {
        public AFoo(int p1, int p2)
        { }

        public abstract void Foo(int p);
        public abstract int Bar();

        protected abstract void Baz(int p);
        protected abstract int Quux();

        public void InvokeBaz(int p) => Baz(p);
        public int InvokeQuux() => Quux();
    }

    // partial keywork is mandatory
    [GenerateMock]
    public partial class MockIFoo : IFoo { }

    // partial keywork is mandatory
    [GenerateMock]
    public partial class MockAFoo : AFoo { }

    public class SomeTests
    {
        [Fact]
        public void TestInterface()
        {
            const int expected = 123;
            var context = new MockContext<IFoo>();
            // interface implementated  explicitly
            IFoo mock = new MockIFoo(context);

            context.Arrange(f => f.Bar()).Returns(expected);
            Assert.Equal(expected, mock.Bar());
            context.Assert(f => f.Bar());
        }

        [Fact]
        public void TestAbstractClass()
        {
            const int expected = 123;
            var context = new MockContext<AFoo>();
            var protectedContext = new MockContext<IP2P_AFoo>();
            // Corresponsing constructor generated
            AFoo mock = new MockAFoo(context, protectedContext, 456, 789);

            // you can use basic or protected context to arrange or assert
            protectedContext.Arrange(f => f.Quux()).Returns(expected);
            Assert.Equal(expected, mock.InvokeQuux());
            protectedContext.Assert(f => f.Quux());

            mock.Foo(expected);
            context.Assert(f => f.Foo(expected));
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            var tests = new SomeTests();
            tests.TestInterface();
            tests.TestAbstractClass();

            Console.WriteLine("Hello World!");
        }
    }
}
```
