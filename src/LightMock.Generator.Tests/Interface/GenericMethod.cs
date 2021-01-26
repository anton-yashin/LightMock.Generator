using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IGenericMethod
    {
        T GenericReturn<T>();
        void GenericParam<T>(T p);
        void GenericWithClassConstraint<T>(T? p) where T : class;
        void GenericWithStructConstraint<T>(T p) where T : struct;
        void GenericWithConstraint<T>(T p) where T : IEquatable<T>, new();
        void GenericWithManyConstraints<T1, T2, T3>(T1? p1, T2 p2, T3 p3)
            where T1 : class, new()
            where T2 : struct, IEquatable<T2>
            where T3 : IEquatable<T3>, new();
    }
}
