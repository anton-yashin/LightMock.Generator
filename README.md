# LightMock.Generator (Beta)

Source generator that generates mocks by provided interfaces. 

## How to use
* Install [LightMock](https://github.com/seesharper/LightMock) and LightMock.Generator to your test project
* Create mock partial class that implement an interface and decorate it with attribute [GenerateMock]

## Example

```csharp
namespace SomeNamespace
{
    public interface IFoo
    {
	    void DoFoo();
    }

    [GenerateMock]
    public partial MockFoo : IFoo { }
}
```

## What will be generated in code behind:

```csharp
using LightMock;

namespace SomeNamespace
{
    partial class MockFoo
    {
        private readonly IInvocationContext<IFoo> context;

        public MockFoo(IInvocationContext<IFoo> context)
        {
            this.context = context;
        }

        void IFoo.DoFoo() { context.Invoke(f => f.DoFoo()); } 
    }
}
```

