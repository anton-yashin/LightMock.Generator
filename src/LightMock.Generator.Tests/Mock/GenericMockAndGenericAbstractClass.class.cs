using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericMockAndGenericAbstractClass<T> : ITestScript<AGenericMockAndGenericAbstractClass<T>>
    {
        private readonly Mock<AGenericMockAndGenericAbstractClass<T>> mock;

        public GenericMockAndGenericAbstractClass()
            => this.mock = new Mock<AGenericMockAndGenericAbstractClass<T>>();

        public MockContext<AGenericMockAndGenericAbstractClass<T>> Context => mock;

        public AGenericMockAndGenericAbstractClass<T> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
