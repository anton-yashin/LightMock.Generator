using System;

namespace LightMock.Generator.Tests.Mock
{
    public interface IInterfaceWithGenericMethod
    {
        T GenericReturn<T>();
        void GenericParam<T>(T p);
        void GenericWithConstraint<T>(T p) where T : new();
    }
}
