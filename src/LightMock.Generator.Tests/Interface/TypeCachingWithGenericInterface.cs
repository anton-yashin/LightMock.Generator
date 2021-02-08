using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface ITypeCachingWithGenericInterface<T>
    {
        T GetSomething();
        void DoSomething(T p);
    }
}
