using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAssertSet
    {
        public abstract string GetAndSet { get; set; }
        public abstract string SetOnly { set; }
    }
}
