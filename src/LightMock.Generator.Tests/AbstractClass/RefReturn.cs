using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ARefReturn
    {
        public abstract ref string Foo();
        public abstract ref readonly DateTime Bar();
    }
}
