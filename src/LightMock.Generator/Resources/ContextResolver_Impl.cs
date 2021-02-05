﻿// <auto-generated />
using System;

namespace LightMock.Generator
{
    public static partial class ContextResolver
    {
        public static Type GetInstanceType(Type contextType)
        {
            var gtd = contextType.IsGenericType ? contextType.GetGenericTypeDefinition() : null;

            /*getInstanceTypeBuilder*/

            throw new NotSupportedException(contextType.FullName + " is not supported " + gtd?.FullName);
        }

        public static Type GetProtectedContextType(Type contextType)
        {
            var gtd = contextType.IsGenericType ? contextType.GetGenericTypeDefinition() : null;

            /*getProtectedContextTypeBuilder*/

            return DefaultProtectedContextType;
        }

        public static Type GetPropertiesContextType(Type contextType)
        {
            var gtd = contextType.IsGenericType ? contextType.GetGenericTypeDefinition() : null;

            /*getPropertiesContextTypeBuilder*/

            if (contextType.IsSubclassOf(MulticastDelegateType))
                return typeof(MockContext<object>);
            throw new NotSupportedException(contextType.FullName + " is not supported " + gtd?.FullName);
        }

        public static Type GetAssertType(Type contextType)
        {
            var gtd = contextType.IsGenericType ? contextType.GetGenericTypeDefinition() : null;

            /*getAssertTypeBuilder*/

            throw new NotSupportedException(contextType.FullName + " is not supported " + gtd?.FullName);
        }

        public static object GetDelegate(Type contextType, object mockContext)
        {
            var gtd = contextType.IsGenericType ? contextType.GetGenericTypeDefinition() : null;

            /*getDelegateBuilder*/

            throw new NotSupportedException(contextType.FullName + " is not supported " + gtd?.FullName);
        }
    }

}
