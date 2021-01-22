using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IFoo
    {
        void Foo();
    }

    public interface IBar : IFoo
    {
        void Bar();
    }

    public interface IBaz
    {
        void Baz();
    }


    public interface IInherittedInterface : IBar, IBaz
    {
        void Quux();
    }
}
