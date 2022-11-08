using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AMethodWithRefParameter
    {
        public abstract string Foo(ref string bar);
    }
}
