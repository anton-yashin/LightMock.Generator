using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IMethodWithInParameter
    {
        string Foo(in string bar);
    }
}
