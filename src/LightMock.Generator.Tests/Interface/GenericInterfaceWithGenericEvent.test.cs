using System;

namespace LightMock.Generator.Tests.Interface
{
    public class GenericInterfaceWithGenericEvent : ITestScript<IGenericInterfaceWithGenericEvent<int>>
    {
        private readonly Mock<IGenericInterfaceWithGenericEvent<int>> mock;

        public GenericInterfaceWithGenericEvent()
            => mock = new Mock<IGenericInterfaceWithGenericEvent<int>>();

        public IMock<IGenericInterfaceWithGenericEvent<int>> Context => mock;

        public IGenericInterfaceWithGenericEvent<int> MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
