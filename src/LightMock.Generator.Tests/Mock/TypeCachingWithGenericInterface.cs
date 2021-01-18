using System;

namespace LightMock.Generator.Tests.Mock
{
    public interface ITypeCachingWithGenericInterface<T>
    {
        T GetSomething();
        void DoSomething(T p);
    }
}
