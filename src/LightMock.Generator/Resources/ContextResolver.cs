using System;

namespace LightMock.Generator
{
    public static partial class ContextResolver
    {
        readonly static Type DefaultProtectedContextType = typeof(object);
        readonly static Type MockContextType = typeof(MockContext<>);
    }
}
