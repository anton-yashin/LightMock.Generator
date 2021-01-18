using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public abstract class ABasicMethod
    {
        public abstract void DoSomething(int p);
        public abstract int GetSomething();

        public void NonAbstractNonVirtualMethod() => throw new NotImplementedException();

        protected abstract void ProtectedDoSomething(int p);
        protected abstract int ProtectedGetSomething();

        protected void ProtectedNonAbstractNonVirtualMethod() => throw new NotImplementedException();
    }
}
