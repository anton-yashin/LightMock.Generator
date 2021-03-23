using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAssertSet_WhenAny
    {
        public abstract string GetAndSet { get; set; }
        public abstract string SetOnly { set; }

        protected abstract string ProtectedGetAndSet { get; set; }
        protected abstract string ProtectedSetOnly { set; }

        public string InvokeProtectedGetAndSet { get => ProtectedGetAndSet; set => ProtectedGetAndSet = value; }
        public string InvokeProtectedSetOnly { set => ProtectedSetOnly = value; }
    }
}
