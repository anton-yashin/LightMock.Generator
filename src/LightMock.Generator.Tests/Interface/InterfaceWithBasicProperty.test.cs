using System;

namespace LightMock.Generator.Tests.Interface
{
    public class InterfaceWithBasicProperty : ITestScript<IInterfaceWithBasicProperty>
    {
        private readonly Mock<IInterfaceWithBasicProperty> mock;

        public InterfaceWithBasicProperty()
        {
            mock = new Mock<IInterfaceWithBasicProperty>();
        }

        public IMock<IInterfaceWithBasicProperty> Context => mock;

        public IInterfaceWithBasicProperty MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
