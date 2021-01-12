using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class AbstractClassWithBasicMethods : ITestScript<AAbstractClassWithBasicMethods>
    {
        private readonly Mock<AAbstractClassWithBasicMethods> mock;

        public AbstractClassWithBasicMethods()
            => mock = new Mock<AAbstractClassWithBasicMethods>();

        public MockContext<AAbstractClassWithBasicMethods> Context => mock;

        public AAbstractClassWithBasicMethods MockObject => mock.Object;

        public int DoRun()
        {
            mock.ProtectedArrange(f => f.ProtectedGetSomething()).Returns(1234);
            Assert.Equal(expected: 1234, mock.Object.InvokeProtectedGetSomething());

            mock.Object.InvokeProtectedDoSomething(5678);
            mock.ProtectedAssert(f => f.ProtectedDoSomething(5678));

            return 42;
        }
    }
}
