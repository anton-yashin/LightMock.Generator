using System;

namespace LightMock.Generator.Tests.Mock
{
    using LightMock;
    using Namespace2;

    public sealed class InterfaceWithMultipleNamespaces : ITestScript<IInterfaceWithMultipleNamespaces>
    {
        private readonly Mock<IInterfaceWithMultipleNamespaces> mock;

        public InterfaceWithMultipleNamespaces()
        {
            mock = new Mock<IInterfaceWithMultipleNamespaces>();
        }

        public MockContext<IInterfaceWithMultipleNamespaces> Context => mock;

        public IInterfaceWithMultipleNamespaces MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
