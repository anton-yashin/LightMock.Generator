using System;
using System.Collections.Generic;

namespace LightMock.Generator
{
    public sealed partial class Mock<T> : MockContext<T> where T : class
    {
        T? instance;
        readonly Type contextType;
        readonly object[] prms;
        readonly static object DefaultProtectedContext = new object();
        readonly static object[] DefaultParams = new object[0];

        public Mock()
        {
            contextType = typeof(T);
            prms = DefaultParams;

            ProtectedContext = CreateProtectedContext();
        }

        public Mock(params object[] prms) : this()
        {
            this.prms = prms;
        }

        public T Object => (instance ?? (instance = CreateMockInstance()));
        public object ProtectedContext { get; }

        static readonly Dictionary<Type, Type> mockInstanceTypes = new Dictionary<Type, Type>();
        static readonly Dictionary<Type, Type> protectedContextTypes = new Dictionary<Type, Type>();

        Type GetOrCacheType(Dictionary<Type, Type> cache, Func<Type> typeFactory)
        {
            Type? tgt = null;
            lock (cache)
            {
                if (cache.TryGetValue(contextType, out tgt) == false)
                {
                    tgt = typeFactory();
                    cache.Add(contextType, tgt);
                }
            }
            return tgt;
        }

        object ActivateMockInstance(Type genericType)
        {
            return Activator.CreateInstance(GetOrCacheType(mockInstanceTypes,
                () => genericType.MakeGenericType(contextType.GetGenericArguments())), this)
                ?? throw new InvalidOperationException("can't create mock instance for: " + contextType.FullName);
        }

        object ActivateMockInstanceWithProtectedContext(Type genericType)
        {
            return Activator.CreateInstance(GetOrCacheType(mockInstanceTypes,
                () => genericType.MakeGenericType(
                    contextType.GetGenericArguments())),
                    args: GetArgs())
                ?? throw new InvalidOperationException("can't create mock instance for: " + contextType.FullName);
        }

        object ActivateMockInstanceWithProtectedContext<TContext>()
        {
            return Activator.CreateInstance(typeof(TContext), args: GetArgs())
                ?? throw new InvalidOperationException("can't create mock instance for: " + contextType.FullName);
        }

        object[] GetArgs()
        {
            var args = new object[prms.Length + 2];
            args[0] = this;
            args[1] = ProtectedContext;
            for (int i = 0; i < prms.Length; i++)
                args[i + 2] = prms[i];
            return args;
        }



        static readonly Type MockContextType = typeof(MockContext<>);

        object ActivateProtectedContext(Type genericInterface)
        {
            return Activator.CreateInstance(GetOrCacheType(protectedContextTypes,
                () => MockContextType.MakeGenericType(
                    genericInterface.MakeGenericType(
                        contextType.GetGenericArguments()))))
                ?? throw new InvalidOperationException("can't create protected context for: " + contextType.FullName);
        }
    }

}
