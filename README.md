# LightMock.Generator

Source generator that generates mocks by provided interfaces, abstract classes and delegates. [Available on nuget](https://www.nuget.org/packages/LightMock.Generator/).
You should be familiar with [LightMock](https://github.com/seesharper/LightMock) because this project uses it underhood.

## How to use
Use Mock\<T\> where T is your abstract class, interface or delegate to batch create MockContext\<T\> and mock object.

## Example with interface

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
        string Baz { get; set; }
    }

    public class SomeTests
    {
        [Fact]
        public void Test()
        {
            var mock = new Mock<IFoo>();
            var o = mock.Object; // use Mock<T>.Object property to get mock object

            o.Foo(123);
            mock.Assert(f => f.Foo(123)); // Mock<T> uses MockContext<T> internally. Use it to assert or arrange context.

            o.Baz = "456"; 
            mock.AssertSet(f => f.Baz = The<string>.Is(s => s == "456")); // There methods available to work with properties.
            // See IMock<T>, IAdvancedMockContext<T> and IMockContext<T> to completed list

            const int expectedBar = 123;
            mock.Arrange(f => f.Bar()).Returns(expectedBar); // Mock<T> uses MockContext<T> internally. Use it to assert or arrange context.
            Assert.Equal(expectedBar, o.Bar());

            int bazInvokedTimes = 0; // ArrangeSetter without suffix uses AOT transformation. Methods with suffix can be used
            mock.ArrangeSetter_WhenAny(f => f.Baz = "").Callback<string>(s => bazInvokedTimes++); //  without AOT transformations.
            o.Baz = "some random value";
            Assert.Equal(1, bazInvokedTimes);
        }
    }
}

```

## Example with abstract class

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
            // It and corresponding interface will be generated only for classes
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

## Example with delegate
```csharp
using LightMock;
using LightMock.Generator;
using System;
using Xunit;

namespace Playground
{
    public class SomeTests
    {
        [Fact]
        public void TestDelegate()
        {
            var expectedObject = new object();
            var expectedArgs = new EventArgs();
            var mock = new Mock<EventHandler>();

            // don't use f => f(args), because LightMock doesn't support that.
            mock.Assert(f => f.Invoke(The<object>.IsAnyValue, The<EventArgs>.IsAnyValue), Invoked.Never);
            mock.Object(expectedObject, expectedArgs);
            mock.Assert(f => f.Invoke(The<object>.IsAnyValue, The<EventArgs>.IsAnyValue));
            mock.Assert(f => f.Invoke(expectedObject, expectedArgs));
        }
    }
}

```

## Additional information

### DisableCodeGenerationAttribute
Place the attribute to your assembly to disable the source code generator.
It can be useful if you moving mocks to separate assembly. Be aware you can't use methods
ArrangeSetter and AssertSet of Mock<T>, because they use AOT transformations.

### DontOverrideAttribute
Use the attribute with class type whose virtual members should not be overridden

### LightMockGenerator_Enable
Use the compiler property in your csproj file with "false" value to disable the source code generator.
It can be useful if you moving mocks to separate assembly. Be aware: the compiler property 
will work if you install a nuget package of the generator into your project.