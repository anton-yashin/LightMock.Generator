using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ABasicMethod
    {
        protected abstract void DoSomething(int p);
        public abstract int GetSomething();

        public void NonAbstractNonVirtualMethod() => throw new NotImplementedException();
    }
}
