using System;

namespace LightMock.Generator.Tests.Mock
{
    public class InterfaceWithEventSource : ITestScript<IInterfaceWithEventSource>
    {
        private readonly Mock<IInterfaceWithEventSource> mock;

        public InterfaceWithEventSource()
        {
            mock = new Mock<IInterfaceWithEventSource>();
        }

        public IMock<IInterfaceWithEventSource> Context => mock;

        public IInterfaceWithEventSource MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
