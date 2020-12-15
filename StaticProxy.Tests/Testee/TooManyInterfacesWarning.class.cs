using System;

namespace StaticProxy.Tests.Testee
{
    interface IFirst { }
    interface ISecond { }

    [GenerateMock]
    partial class TooManyInterfacesWarning : IFirst, ISecond
    {
    }
}
