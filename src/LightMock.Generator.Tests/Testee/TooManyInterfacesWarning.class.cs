using System;

namespace LightMock.Generator.Tests.Testee
{
    interface IFirst { }
    interface ISecond { }

    [GenerateMock]
    partial class TooManyInterfacesWarning : IFirst, ISecond
    {
    }
}
