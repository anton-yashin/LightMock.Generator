using System;

namespace LightMock.Generator
{
    public sealed partial class Mock<T> : MockContext<T> where T : class
    {
        Type GetInstanceType()
            => throw new NotSupportedException(typeof(T).FullName + " is not supported");

        Type GetProtectedContextType()
            => MockDefaults.DefaultProtectedContextType;
    }
}
