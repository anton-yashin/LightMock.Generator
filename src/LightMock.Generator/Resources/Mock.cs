﻿using System;
using System.Threading;

namespace LightMock.Generator
{
    static class MockDefaults
    {
        public readonly static Type DefaultProtectedContextType = typeof(object);
        public readonly static object[] DefaultParams = Array.Empty<object>();
        public readonly static Type MockContextType = typeof(MockContext<>);
    }

    public sealed partial class Mock<T> : MockContext<T> where T : class
    {
        T? instance;
        readonly object[] prms;

        public Mock()
        {
            prms = MockDefaults.DefaultParams;

            ProtectedContext = CreateProtectedContext();
        }

        public Mock(params object[] prms) : this()
        {
            this.prms = prms;
        }

        public T Object => (instance ??= CreateMockInstance());
        public object ProtectedContext { get; }

        static Type? mockInstanceType;
        static Type? protectedContextType;

        object[] GetArgs()
        {
            var args = new object[prms.Length + 2];
            args[0] = this;
            args[1] = ProtectedContext;
            for (int i = 0; i < prms.Length; i++)
                args[i + 2] = prms[i];
            return args;
        }

        T CreateMockInstance()
        {
            var result = Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref mockInstanceType,
                GetInstanceType), args: GetArgs())
                ?? throw new InvalidOperationException("can't create context for: " + typeof(T).FullName);
            return (T)result;
        }

        object CreateProtectedContext()
        {
            return Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref protectedContextType,
                GetProtectedContextType))
                ?? throw new InvalidOperationException("can't create protected context for: " + typeof(T).FullName);
        }
    }

}
