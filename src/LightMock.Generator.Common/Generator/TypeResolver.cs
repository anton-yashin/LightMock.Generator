using LightMock.Generator.Tags;
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
            => this.ContextType = contextType;

        public virtual Type GetInstanceType()
            => throw new NotSupportedException();

        public virtual Type GetProtectedContextType()
            => Defaults.DefaultProtectedContextType;

        public virtual Type GetPropertiesContextType()
            => ContextType.IsSubclassOf(Defaults.MulticastDelegateType)
                ? Defaults.MulticastDelegateContextType
                : throw new NotSupportedException();

        public virtual Type GetAssertType()
            => throw new NotSupportedException();

        public virtual Type GetAssertIsAnyType()
            => throw new NotSupportedException();

        public virtual Type GetArrangeOnAnyType()
            => throw new NotSupportedException();

        public virtual Type GetArrangeOnType()
            => throw new NotSupportedException();

        public virtual object GetDelegate(object mockContext)
            => throw new NotSupportedException();

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

        internal TMock ActivateInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, MockInstanceTag)>().Activate(GetInstanceType, args);

        internal object ActivateProtectedContext<TMock>()
            => new TypeCacher<(TMock, ProtectedContextTag)>().Activate(GetProtectedContextType);

        internal IMockContextInternal ActivatePropertiesContext<TMock>()
            => (IMockContextInternal)new TypeCacher<(TMock, PropertiesContextTag)>().Activate(GetPropertiesContextType);

        internal TMock ActivateAssertInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, AssertTag)>().Activate(GetAssertType, args);

        internal TMock ActivateAssertIsAnyInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, AssertAnyTag)>().Activate(GetAssertIsAnyType, args);

        internal TMock ActivateArrangeOnAnyInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, ArrangeAnyTag)>().Activate(GetArrangeOnAnyType, args);

        internal TMock ActivateArrangeOnInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, ArrangeTag)>().Activate(GetArrangeOnType, args);
    }
}
