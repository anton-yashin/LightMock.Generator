using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericInterfaceWithVariance : ITestScript<IGenericInterfaceWithVariance<IFoo, IBar>>
    {
        private readonly Mock<IGenericInterfaceWithVariance<IFoo, IBar>> mock;

        public GenericInterfaceWithVariance()
            => mock = new Mock<IGenericInterfaceWithVariance<IFoo, IBar>>();

        public MockContext<IGenericInterfaceWithVariance<IFoo, IBar>> Context => mock;

        public IGenericInterfaceWithVariance<IFoo, IBar> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
