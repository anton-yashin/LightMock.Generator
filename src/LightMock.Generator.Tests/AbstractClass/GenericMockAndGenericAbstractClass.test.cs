using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class GenericMockAndGenericAbstractClass<T> : ITestScript<AGenericMockAndGenericAbstractClass<T>>
        where T : struct, IComparable<T>
    {
        private readonly Mock<AGenericMockAndGenericAbstractClass<T>> mock;

        public GenericMockAndGenericAbstractClass()
            => this.mock = new Mock<AGenericMockAndGenericAbstractClass<T>>();

        public IMock<AGenericMockAndGenericAbstractClass<T>> Context => mock;

        public AGenericMockAndGenericAbstractClass<T> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
