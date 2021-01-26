using System;

namespace LightMock.Generator.Tests.Mock
{

    public interface IGenericInterfaceWithVariance<in T1, out T2>
    {
        void Foo();
    }
}
