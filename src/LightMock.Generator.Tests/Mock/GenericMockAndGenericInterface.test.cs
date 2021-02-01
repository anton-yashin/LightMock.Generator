using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericMockAndGenericInterface<T> : ITestScript<IGenericMockAndGenericInterface<T>>
        where T : struct, System.IComparable<T>
    {
        private readonly Mock<IGenericMockAndGenericInterface<T>> mock;

        public GenericMockAndGenericInterface()
            => this.mock = new Mock<IGenericMockAndGenericInterface<T>>();

        public IMock<IGenericMockAndGenericInterface<T>> Context => mock;

        public IGenericMockAndGenericInterface<T> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
