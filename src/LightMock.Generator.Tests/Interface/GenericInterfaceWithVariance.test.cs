using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    public class GenericInterfaceWithVariance : ITestScript<IGenericInterfaceWithVariance<IFoo, IBar>>
    {
        private readonly Mock<IGenericInterfaceWithVariance<IFoo, IBar>> mock;

        public GenericInterfaceWithVariance()
            => mock = new Mock<IGenericInterfaceWithVariance<IFoo, IBar>>();

        public IMock<IGenericInterfaceWithVariance<IFoo, IBar>> Context => mock;

        public IGenericInterfaceWithVariance<IFoo, IBar> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
