using System;
using System.Collections.Generic;
using System.Text;

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

            return MockDefaults.DefaultProtectedContextType;
        }

    }

}
