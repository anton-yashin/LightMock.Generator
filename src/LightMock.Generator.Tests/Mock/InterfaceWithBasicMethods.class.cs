using System;
using LightMock.Generator;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class InterfaceWithBasicMethods : ITestScript<IInterfaceWithBasicMethods>
    {
        private readonly Mock<IInterfaceWithBasicMethods> mock;

        public InterfaceWithBasicMethods()
        {
            mock = new Mock<IInterfaceWithBasicMethods>();
        }

        public MockContext<IInterfaceWithBasicMethods> Context => mock;
        public IInterfaceWithBasicMethods MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
