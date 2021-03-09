using System;
using System.ComponentModel;

namespace LightMock.Generator
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class TypeResolver
    {
        protected Type ContextType { get; }
        protected IContextResolverDefaults Defaults => ContextResolverDefaults.Instance;

        protected TypeResolver(Type contextType)
        {
            this.ContextType = contextType;
        }

        public virtual Type GetInstanceType()
        {
            throw new global::System.NotSupportedException();
        }

        public virtual Type GetProtectedContextType()
        {
            return Defaults.DefaultProtectedContextType;
        }

        public virtual Type GetPropertiesContextType()
        {
            if (ContextType.IsSubclassOf(Defaults.MulticastDelegateType))
                return Defaults.MulticastDelegateContextType;

            throw new global::System.NotSupportedException();
        }

        public virtual Type GetAssertType()
        {
            throw new global::System.NotSupportedException();
        }

        public virtual Type GetAssertIsAnyType()
        {
            throw new global::System.NotSupportedException();
        }

        public virtual Type GetArrangeOnAnyType()
        {
            throw new NotSupportedException();
        }

        public virtual object GetDelegate(object mockContext)
        {
            throw new global::System.NotSupportedException();
        }

        protected object CreateGenericDelegate(Type delegateType, object mockContext, string fullName)
        {
            var dp = Activator.CreateInstance(delegateType.MakeGenericType(ContextType.GetGenericArguments()), new object[] { mockContext } )
                ?? throw new InvalidOperationException($"can't create delegate for {fullName}");
            return ((IDelegateProvider)dp).GetDelegate();
        }

        protected Type MakeGenericType(Type genericType) 
            => genericType.MakeGenericType(ContextType.GetGenericArguments());

        protected Type MakeMockContextType(Type type)
            => Defaults.MockContextType.MakeGenericType(type);

        protected Type MakeGenericMockContextType(Type genericType)
            => Defaults.MockContextType.MakeGenericType(genericType.MakeGenericType(ContextType.GetGenericArguments()));
    }
}
