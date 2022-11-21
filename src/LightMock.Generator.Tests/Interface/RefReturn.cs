using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IRefReturn
    {
        ref string Foo();
        ref readonly DateTime Bar();
    }
}
