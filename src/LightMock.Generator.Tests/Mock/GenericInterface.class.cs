using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericInterface : ITestScript<IGenericInterface<int>>
    {
        private readonly Mock<IGenericInterface<int>> mock;

        public GenericInterface()
        {
            mock = new Mock<IGenericInterface<int>>();
        }

        public MockContext<IGenericInterface<int>> Context => mock;

        public IGenericInterface<int> MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
