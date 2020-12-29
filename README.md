# LightMock.Generator (Beta)

Source generator that generates mocks by provided interfaces and classes. [Available on nuget](https://www.nuget.org/packages/LightMock.Generator/)

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

    public abstract class AbstractClass
    {
        public abstract void DoFoo();
        protected abstract void DoProtectedFoo();
    }

    [GenerateMock]
    public partial MockFoo : IFoo { }

    [GenerateMock]
    public partial MockAbstractClass : AbstractClass { }
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

    public interface IP2P_AbstractClass
    {
        void DoProtectedFoo();
    }

    partial class MockAbstractClass : IP2P_AbstractClass
    {
        private readonly IInvocationContext<AbstractClass> context;
        private readonly IInvocationContext<IP2P_AbstractClass> protectedContext;

        public BasicMethod(IInvocationContext<AbstractClass> context, IInvocationContext<IP2P_AbstractClass> protectedContext)
        {
            this.context = context;
            this.protectedContext = protectedContext;
        }

        override public void DoFoo(int p)
        {
            context.Invoke(f => f.DoFoo(p));
        }

        void LightMockIP2P_ABasicMethod.ProtectedDoFoo()
        {
            protectedContext.Invoke(f => f.ProtectedDoFoo());
        }

        override protected void ProtectedDoFoo()
        {
            protectedContext.Invoke(f => f.ProtectedDoFoo());
        }
    }
}
```

