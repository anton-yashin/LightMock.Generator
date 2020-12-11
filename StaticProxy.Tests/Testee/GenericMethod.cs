using System;

namespace StaticProxy.Tests.Testee
{
    interface IGenericMethod
    {
        T GenericReturn<T>();
        void GenericParam<T>(T p);
        void GenericWithConstraint<T>(T p) where T : new();
    }


    [GenerateMock]
    partial class GenericMethod : IGenericMethod
    {
    }
}
