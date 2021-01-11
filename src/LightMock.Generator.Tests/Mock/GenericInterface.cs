using System;

namespace LightMock.Generator.Tests.Mock
{
    public interface IGenericInterface<T>
    {
        T OnlyGet { get; }
        T GetAndSet { get; set; }
        T GetSomething();
        void DoSomething(T p);
    }
}
