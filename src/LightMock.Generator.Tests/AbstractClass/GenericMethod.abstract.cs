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
        public abstract void GenericWithConstraint<T>(T? p) where T : class;

        protected abstract T ProtectedGenericReturn<T>();
        protected abstract void ProtectedGenericParam<T>(T p);
        protected abstract void ProtectedGenericWithConstraint<T>(T? p) where T : class;
    }
}
