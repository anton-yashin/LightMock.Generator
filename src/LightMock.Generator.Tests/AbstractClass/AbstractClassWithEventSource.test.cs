using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassWithEventSource : ITestScript<AAbstractClassWithEventSource>
    {
        private readonly Mock<AAbstractClassWithEventSource> mock;

        public AbstractClassWithEventSource()
            => mock = new Mock<AAbstractClassWithEventSource>();

        public IMock<AAbstractClassWithEventSource> Context => mock;

        public AAbstractClassWithEventSource MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
