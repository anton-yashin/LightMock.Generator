using System;

namespace LightMock.Generator.Tests.Interface
{
    interface IFirst { }
    interface ISecond { }

    [GenerateMock]
    partial class TooManyInterfacesWarning : IFirst, ISecond
    {
    }
}
