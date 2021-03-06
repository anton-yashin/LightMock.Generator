﻿using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Interface
{
    public interface IInterfaceWithGenericMethod
    {
        T GenericReturn<T>();
        Task<T?> GenericReturnAsync<T>() where T : class;
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
