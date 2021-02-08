using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    using EventNamespace2;

    public class AbstractClassWithEventSourceAndMultipleNamespaces : ITestScript<AAbstractClassWithEventSourceAndMultipleNamespaces>
    {
        private readonly Mock<AAbstractClassWithEventSourceAndMultipleNamespaces> mock;

        public AbstractClassWithEventSourceAndMultipleNamespaces()
            => mock = new Mock<AAbstractClassWithEventSourceAndMultipleNamespaces>();

        public IMock<AAbstractClassWithEventSourceAndMultipleNamespaces> Context => mock;

        public AAbstractClassWithEventSourceAndMultipleNamespaces MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
