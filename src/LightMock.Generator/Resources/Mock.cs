using System;

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
    }

}
