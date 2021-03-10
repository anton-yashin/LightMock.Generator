using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AArrangeSetter_OnAny
    {
        public abstract string GetAndSet { get; set; }
        public abstract string SetOnly { set; }
    }
}
