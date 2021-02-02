using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Mock
{
    public abstract class AAbstractClassWithTaskMethod
    {
        public abstract Task FooAsync();
        public abstract Task<int> BarAsync();
        public abstract Task<T> BazAsync<T>();

        public virtual Task VirtualFooAsync() => null!;
        public virtual Task<int> VirtualBarAsync() => null!;
        public virtual Task<T> VirtualBazAsync<T>() => null!;

        protected abstract Task ProtectedFooAsync();
        protected abstract Task<int> ProtectedBarAsync();
        protected abstract Task<T> ProtectedBazAsync<T>();

        protected virtual Task ProtectedVirtualFooAsync() => null!;
        protected virtual Task<int> ProtectedVirtualBarAsync() => null!;
        protected virtual Task<T> ProtectedVirtualBazAsync<T>() => null!;

        public Task InvokeProtectedFooAsync() => ProtectedFooAsync();
        public Task<int> InvokeProtectedBarAsync() => ProtectedBarAsync();
        public Task<T> InvokeProtectedBazAsync<T>()=> ProtectedBazAsync<T>();

        public Task InvokeProtectedVirtualFooAsync() => ProtectedVirtualFooAsync();
        public Task<int> InvokeProtectedVirtualBarAsync() => ProtectedVirtualBarAsync();
        public Task<T> InvokeProtectedVirtualBazAsync<T>() => ProtectedVirtualBazAsync<T>();

    }
}
