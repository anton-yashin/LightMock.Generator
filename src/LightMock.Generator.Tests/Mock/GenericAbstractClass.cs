using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Mock
{
    public abstract class AGenericAbstractClass<T>
    {
        public abstract T OnlyGet { get; }
        public abstract T GetAndSet { get; set; }
        public abstract T GetSomething();
        public abstract void DoSomething(T p);
        public abstract Task<TResult?> FooAsync<TResult>() where TResult : class;

        protected abstract T ProtectedOnlyGet { get; }
        protected abstract T ProtectedGetAndSet { get; set; }
        protected abstract T ProtectedGetSomething();
        protected abstract void ProtectedDoSomething(T p);
        public abstract Task<TResult?> ProtectedFooAsync<TResult>() where TResult : class;


        public T InvokeProtectedOnlyGet => ProtectedOnlyGet;
        public T InvokeProtectedGetAndSet
        {
            get => ProtectedGetAndSet;
            set => ProtectedGetAndSet = value;
        }

        public T InvokeProtectedGetSomething() => ProtectedGetSomething();
        public void InvokeProtectedDoSomething(T p) => ProtectedDoSomething(p);

    }
}
