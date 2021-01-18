using System;

namespace LightMock.Generator.Tests.Mock
{
    public abstract class ATypeCachingWithAbstractClass
    {
        public abstract void DoSomething(int p);
        public abstract int GetSomething();

        public void NonAbstractNonVirtualMethod() => throw new NotImplementedException();

        protected abstract void ProtectedDoSomething(int p);
        protected abstract int ProtectedGetSomething();

        public void InvokeProtectedDoSomething(int p) => ProtectedDoSomething(p);
        public int InvokeProtectedGetSomething() => ProtectedGetSomething();

        protected void ProtectedNonAbstractNonVirtualMethod() => throw new NotImplementedException();
    }
}
