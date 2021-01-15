using System;

namespace LightMock.Generator
{
    public sealed partial class Mock<T> : MockContext<T> where T : class
    {
        T CreateMockInstance()
            => throw new NotSupportedException(contextType.FullName + " is not supported");

        object CreateProtectedContext()
            => DefaultProtectedContext;
    }
}
