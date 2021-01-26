using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AGenericMethod
    {
        public abstract T GenericReturn<T>();
        public abstract void GenericParam<T>(T p);
        public abstract void GenericWithClassConstraint<T>(T? p) where T : class;
        public abstract void GenericWithStructConstraint<T>(T p) where T : struct;
        public abstract void GenericWithConstraint<T>(T p) where T : IEquatable<T>, new();
        public abstract void GenericWithManyConstraints<T1, T2, T3>(T1? p1, T2 p2, T3 p3)
            where T1 : class, new()
            where T2 : struct, IEquatable<T2>
            where T3 : IEquatable<T3>, new();

        protected abstract T ProtectedGenericReturn<T>();
        protected abstract void ProtectedGenericParam<T>(T p);
        protected abstract void ProtectedGenericWithClassConstraint<T>(T? p) where T : class;
        protected abstract void ProtectedGenericWithStructConstraint<T>(T p) where T : struct;
        protected abstract void ProtectedGenericWithConstraint<T>(T p) where T : IEquatable<T>, new();
        protected abstract void ProtectedGenericWithManyConstraints<T1, T2, T3>(T1? p1, T2 p2, T3 p3)
            where T1 : class, new()
            where T2 : struct, IEquatable<T2>
            where T3 : IEquatable<T3>, new();
    }
}
