using System;

namespace LightMock.Generator.Tests.Mock
{
    using EventNamespace2;

    public class AbstractClassWithEventSourceAndMultipleNamespaces : ITestScript<AAbstractClassWithEventSourceAndMultipleNamespaces>
    {
        private readonly Mock<AAbstractClassWithEventSourceAndMultipleNamespaces> mock;

        public AbstractClassWithEventSourceAndMultipleNamespaces()
            => mock = new Mock<AAbstractClassWithEventSourceAndMultipleNamespaces>();

        public MockContext<AAbstractClassWithEventSourceAndMultipleNamespaces> Context => mock;

        public AAbstractClassWithEventSourceAndMultipleNamespaces MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
