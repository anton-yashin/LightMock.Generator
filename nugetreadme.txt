# LightMock.Generator (Beta)

## Source generator that generates mocks by provided interfaces. 

How to use:
* Don't forget to install LightMock https://github.com/seesharper/LightMock
* Create mock partial class that implement an interface and decorate it with attribute [GenerateMock]

## Example

namespace SomeNamespace
{
    // your inteface
    public interface IFoo
    {
        void DoFoo();
    }

    // your mock
    [GenerateMock]
    public partial MockFoo : IFoo { }
}

## What will be generated in code behind:

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
