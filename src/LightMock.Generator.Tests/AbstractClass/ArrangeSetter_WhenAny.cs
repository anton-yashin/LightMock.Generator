using System;
using System.Collections.Generic;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AArrangeSetter_WhenAny
    {
        protected AArrangeSetter_WhenAny(object a1, int a2, IEnumerable<object> a3) { }

        public abstract string GetAndSet { get; set; }
        public abstract string SetOnly { set; }
    }
}
