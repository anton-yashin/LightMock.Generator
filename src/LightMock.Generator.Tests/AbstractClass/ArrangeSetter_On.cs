using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AArrangeSetter_On
    {
        protected AArrangeSetter_On(object a1, int a2) { }

        public abstract string GetAndSet { get; set; }
        public abstract string SetOnly { set; }

    }
}
