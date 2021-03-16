﻿/******************************************************************************
    MIT License

    Copyright (c) 2021 Anton Yashin

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*******************************************************************************
    https://github.com/anton-yashin/
*******************************************************************************/

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

        public virtual Type GetAssertWhenType()
            => throw new NotSupportedException();

        public virtual Type GetAssertWhenAnyType()
            => throw new NotSupportedException();

        public virtual Type GetArrangeWhenAnyType()
            => throw new NotSupportedException();

        public virtual Type GetArrangeWhenType()
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

        internal TMock ActivateAssertWhenInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, AssertWhenTag)>().Activate(GetAssertWhenType, args);

        internal TMock ActivateAssertWhenAnyInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, AssertWhenAnyTag)>().Activate(GetAssertWhenAnyType, args);

        internal TMock ActivateArrangeWhenAnyInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, ArrangeWhenAnyTag)>().Activate(GetArrangeWhenAnyType, args);

        internal TMock ActivateArrangeWhenInstance<TMock>(object[] args)
            => (TMock)new TypeCacher<(TMock, ArrangeWhenTag)>().Activate(GetArrangeWhenType, args);
    }
}