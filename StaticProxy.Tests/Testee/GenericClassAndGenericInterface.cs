using System;

namespace StaticProxy.Tests.Testee
{
    interface IGenericClassAndGenericInterface<T>
    {
        T OnlyGet { get; }
        T GetAndSet { get; set; }
        T GetSomething();
        void DoSomething(T p);
    }

    [GenerateMock]
    partial class GenericClassAndGenericInterface<T> : IGenericClassAndGenericInterface<T>
    {
    }
}
