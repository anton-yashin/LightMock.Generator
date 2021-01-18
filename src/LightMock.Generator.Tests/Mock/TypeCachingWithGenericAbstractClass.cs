using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public abstract class ATypeCachingWithGenericAbstractClass<T>
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

    public class TypeCachingWithGenericAbstractClass : ITestScript<ATypeCachingWithGenericAbstractClass<int>>
    {
        private readonly Mock<ATypeCachingWithGenericAbstractClass<int>> mock;

        public TypeCachingWithGenericAbstractClass()
            => mock = new Mock<ATypeCachingWithGenericAbstractClass<int>>();

        public MockContext<ATypeCachingWithGenericAbstractClass<int>> Context => mock;

        public ATypeCachingWithGenericAbstractClass<int> MockObject => mock.Object;

        public int DoRun()
        {
            var another = new Mock<ATypeCachingWithGenericAbstractClass<int>>();
            var o = another.Object;
            Assert.NotNull(o);

            return 42;
        }
    }
}
