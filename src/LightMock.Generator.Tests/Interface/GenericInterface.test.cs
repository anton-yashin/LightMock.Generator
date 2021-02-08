using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    public class GenericInterface : ITestScript<IGenericInterface<int>>
    {
        private readonly Mock<IGenericInterface<int>> mock;

        public GenericInterface()
        {
            mock = new Mock<IGenericInterface<int>>();
        }

        public IMock<IGenericInterface<int>> Context => mock;

        public IGenericInterface<int> MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
