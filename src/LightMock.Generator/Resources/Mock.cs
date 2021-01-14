using System;
using System.Collections.Generic;

namespace LightMock.Generator
{
    public sealed partial class Mock<T> : MockContext<T> where T : class
    {
        T? instance;
        readonly Type contextType;
        readonly static object DefaultProtectedContext = new object();

        public Mock()
        {
            contextType = typeof(T);

            ProtectedContext = CreateProtectedContext();
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
                    this, ProtectedContext)
                ?? throw new InvalidOperationException("can't create mock instance for: " + contextType.FullName);
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
