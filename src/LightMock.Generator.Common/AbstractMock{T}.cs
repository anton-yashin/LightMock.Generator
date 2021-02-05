using System;
using System.Diagnostics;
using System.Threading;

namespace LightMock.Generator
{
    public abstract class AbstractMock<T> : MockContext<T>, IProtectedContext<T>, IMock<T>
        where T : class
    {
        T? instance;
        readonly object[] prms;
        readonly object protectedContext;
        readonly object propertiesContext;

        public AbstractMock()
        {
            prms = Array.Empty<object>();

            protectedContext = CreateProtectedContext();
            propertiesContext = CreatePropertiesContext();
        }

        public AbstractMock(params object[] prms) : this()
        {
            this.prms = prms;
        }

        public T Object => LazyInitializer.EnsureInitialized(ref instance!, CreateMockInstance);

        object IProtectedContext<T>.ProtectedContext => protectedContext;

        static Type? mockInstanceType;
        static Type? protectedContextType;
        static Type? propertiesType;
        static Type? assertType;

        object[] GetArgs()
        {
            const int offset = 3;
            var args = new object[prms.Length + offset];
            args[0] = this;
            args[1] = propertiesContext;
            args[2] = protectedContext;
            for (int i = 0; i < prms.Length; i++)
                args[i + offset] = prms[i];
            return args;
        }

        object[] GetAssertArgs(Invoked invoked)
        {
            var args = new object[prms.Length + 2];
            args[0] = propertiesContext;
            args[1] = invoked;
            for (int i = 0; i < prms.Length; i++)
                args[i + 2] = prms[i];
            return args;
        }

        T CreateMockInstance()
        {
            var type = LazyInitializer.EnsureInitialized(ref mockInstanceType!, GetInstanceType);
            if (type.IsDelegate())
                return GetDelegate(type);
            var result = Activator.CreateInstance(type, args: GetArgs())
                ?? throw new InvalidOperationException("can't create context for: " + typeof(T).FullName);
            return (T)result;
        }

        object CreateProtectedContext()
        {
            return Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref protectedContextType,
                GetProtectedContextType))
                ?? throw new InvalidOperationException("can't create protected context for: " + typeof(T).FullName);
        }

        T CreateAssertInstance(Invoked invoked)
        {
            var result = Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref assertType,
                GetAssertType), args: GetAssertArgs(invoked))
                ?? throw new InvalidOperationException("can't create assert for: " + typeof(T).FullName);
            return (T)result;
        }

        object CreatePropertiesContext()
        {
            return Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref propertiesType,
                GetPropertiesContextType))
                ?? throw new InvalidOperationException("can't create property context for: " + typeof(T).FullName);
        }


        protected abstract Type GetInstanceType();
        protected abstract Type GetProtectedContextType();
        protected abstract Type GetPropertiesContextType();
        protected abstract Type GetAssertType();
        protected abstract T GetDelegate(Type type);

        [DebuggerStepThrough]
        public void AssertGet<TProperty>(Func<T, TProperty> expression)
        {
            AssertGet(expression, Invoked.Once);
        }

        [DebuggerStepThrough]
        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
        {
            expression(CreateAssertInstance(times));
        }

        [DebuggerStepThrough]
        public void AssertSet(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.Once);

        [DebuggerStepThrough]
        public void AssertSet(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        [DebuggerStepThrough]
        public void AssertAdd(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.Once);

        [DebuggerStepThrough]
        public void AssertAdd(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        [DebuggerStepThrough]
        public void AssertRemove(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.Once);

        [DebuggerStepThrough]
        public void AssertRemove(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        void AssertUsingAssertInstance(Action<T> expression, Invoked times)
            => expression(CreateAssertInstance(times));
    }
}
