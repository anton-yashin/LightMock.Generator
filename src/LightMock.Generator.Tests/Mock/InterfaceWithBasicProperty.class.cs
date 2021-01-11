using System;

namespace LightMock.Generator.Tests.Mock
{
    public class InterfaceWithBasicProperty : ITestScript<IInterfaceWithBasicProperty>
    {
        private readonly Mock<IInterfaceWithBasicProperty> mock;

        public InterfaceWithBasicProperty()
        {
            mock = new Mock<IInterfaceWithBasicProperty>();
        }

        public MockContext<IInterfaceWithBasicProperty> Context => mock;

        public IInterfaceWithBasicProperty MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
