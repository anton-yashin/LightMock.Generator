using System;

namespace LightMock.Generator.Tests.Interface
{
    interface IFirst { }

    abstract class ASomeClass { }

    [GenerateMock]
    partial class TooManyInterfacesWarning : ASomeClass, IFirst
    {
    }
}
