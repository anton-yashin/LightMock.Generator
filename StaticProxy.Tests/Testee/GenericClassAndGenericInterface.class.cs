using System;

namespace StaticProxy.Tests.Testee
{
    [GenerateMock]
    public partial class GenericClassAndGenericInterface<T> : IGenericClassAndGenericInterface<T>
    {
    }
}
