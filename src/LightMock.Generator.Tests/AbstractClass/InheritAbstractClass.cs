using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AFoo
    {
        public abstract void Foo();
        protected abstract void ProtectedFoo();
        public void InvokeProtectedFoo() => ProtectedFoo();
    }

    public abstract class ABar : AFoo
    {
        public abstract void Bar();
        protected abstract void ProtectedBar();
        public void InvokeProtectedBar() => ProtectedBar();
    }

    public abstract class AInheritAbstractClass : ABar
    {
        public override void Foo() { }
        protected override void ProtectedFoo() { }

        public abstract void Baz();
        protected abstract void ProtectedBaz();
        public void InvokeProtectedBaz() => ProtectedBaz();
    }

}

