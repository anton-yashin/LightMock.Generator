using System;

namespace LightMock.Generator
{
    static class TypeExtensions
    {
        readonly static Type DelegateType = typeof(Delegate);

        public static bool IsDelegate(this Type @this)
            => @this.IsSubclassOf(DelegateType);
    }
}
