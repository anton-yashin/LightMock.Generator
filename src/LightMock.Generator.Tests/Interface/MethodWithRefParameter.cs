using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IMethodWithRefParameter
    {
        string Foo(ref string bar);
    }
}
