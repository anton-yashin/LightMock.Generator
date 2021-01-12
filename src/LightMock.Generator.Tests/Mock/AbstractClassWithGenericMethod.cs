using System;

namespace LightMock.Generator.Tests.Mock
{
    public abstract class AAbstractClassWithGenericMethod
    {
        public abstract T GenericReturn<T>();
        public abstract void GenericParam<T>(T p);
        public abstract void GenericWithConstraint<T>(T p) where T : new();

        protected abstract T ProtectedGenericReturn<T>();
        protected abstract void ProtectedGenericParam<T>(T p);
        protected abstract void ProtectedGenericWithConstraint<T>(T p) where T : new();
    }
}
