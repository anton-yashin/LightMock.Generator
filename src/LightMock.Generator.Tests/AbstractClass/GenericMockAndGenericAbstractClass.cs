using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AGenericMockAndGenericAbstractClass<T>
        where T : struct, IComparable<T>
    {
        public abstract T OnlyGet { get; }
        public abstract T GetAndSet { get; set; }
        public abstract T GetSomething();
        public abstract void DoSomething(T p);

        protected abstract T ProtectedOnlyGet { get; }
        protected abstract T ProtectedGetAndSet { get; set; }
        protected abstract T ProtectedGetSomething();
        protected abstract void ProtectedDoSomething(T p);


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
