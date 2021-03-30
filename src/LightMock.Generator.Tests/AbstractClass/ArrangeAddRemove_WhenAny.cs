using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AArrangeAddRemove_WhenAny
    {
        public abstract event EventHandler EventHandler;
        protected abstract event EventHandler ProtectedEventHandler;

        public event EventHandler InvokeProtectedEventHandler
        {
            add => ProtectedEventHandler += value;
            remove => ProtectedEventHandler -= value;
        }
    }
}
