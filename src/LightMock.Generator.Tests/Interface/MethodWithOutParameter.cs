using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IMethodWithOutParameter
    {
        int Foo(out int bar);
    }
}
