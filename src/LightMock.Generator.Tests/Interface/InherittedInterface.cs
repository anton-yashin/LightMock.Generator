using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IFooResult { }

    public interface IFoo
    {
        void Foo();
        IFooResult GetResult();
    }

    public interface IBarResult { }

    public interface IBar : IFoo
    {
        void Bar();
        new IBarResult GetResult();
    }

    public interface IBazResult { }

    public interface IBaz
    {
        void Baz();
        IBazResult GetResult();
    }

    public interface IQuuxResult { }

    public interface IInherittedInterface : IBar, IBaz
    {
        void Quux();
        new IQuuxResult GetResult();
    }
}
