using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AMethodWithInParameter
    {
        public abstract string Foo(in string bar);
    }
}
