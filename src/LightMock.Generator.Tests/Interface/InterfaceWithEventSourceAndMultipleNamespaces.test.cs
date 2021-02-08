using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    using EventNamespace2;
    using LightMock;

    public class InterfaceWithEventSourceAndMultipleNamespaces : ITestScript<IInterfaceWithEventSourceAndMultipleNamespaces>
    {
        private readonly Mock<IInterfaceWithEventSourceAndMultipleNamespaces> mock;

        public InterfaceWithEventSourceAndMultipleNamespaces()
            => mock = new Mock<IInterfaceWithEventSourceAndMultipleNamespaces>();

        public IMock<IInterfaceWithEventSourceAndMultipleNamespaces> Context => mock;

        public IInterfaceWithEventSourceAndMultipleNamespaces MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
