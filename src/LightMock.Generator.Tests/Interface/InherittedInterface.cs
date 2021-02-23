using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IFooResult { }

    public interface IFoo
    {
        void Foo();
        IFooResult GetResult();
        IFooResult Result { get; }
    }

    public interface IBarResult { }

    public interface IBar : IFoo
    {
        void Bar();
        new IBarResult GetResult();
        new IBarResult Result { get; }
    }

    public interface IBazResult { }

    public interface IBaz
    {
        void Baz();
        IBazResult GetResult();
        IBazResult Result { get; }
    }

    public interface IQuuxResult { }

    public interface IInherittedInterface : IBar, IBaz
    {
        void Quux();
        new IQuuxResult GetResult();
        new IQuuxResult Result { get; }
    }
}
