using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AGenericClassAndGenericBaseClass<T>
        where T : struct, IEquatable<T>
    {
        public abstract T OnlyGet { get; }
        public abstract T GetAndSet { get; set; }
        public abstract T GetSomething();
        public abstract void DoSomething(T p);
        public abstract Task<TResult> FooAsync<TResult>() where TResult : class;

        protected abstract T ProtectedOnlyGet { get; }
        protected abstract T ProtectedGetAndSet { get; set; }
        protected abstract T ProtectedGetSomething();
        protected abstract void ProtectedDoSomething(T p);
        protected abstract Task<TResult> ProtectedFooAsync<TResult>() where TResult : class;
    }
}
