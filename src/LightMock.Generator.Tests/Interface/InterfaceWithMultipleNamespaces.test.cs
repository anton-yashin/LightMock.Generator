using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
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

        public IMock<IInterfaceWithMultipleNamespaces> Context => mock;

        public IInterfaceWithMultipleNamespaces MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
