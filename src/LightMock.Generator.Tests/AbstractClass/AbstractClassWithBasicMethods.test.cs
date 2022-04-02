using System;
using Xunit;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassWithBasicMethods : ITestScript<AAbstractClassWithBasicMethods>
    {
        private readonly Mock<AAbstractClassWithBasicMethods> mock;

        public AbstractClassWithBasicMethods()
            => mock = new Mock<AAbstractClassWithBasicMethods>();

        public IMock<AAbstractClassWithBasicMethods> Context => mock;

        public AAbstractClassWithBasicMethods MockObject => mock.Object;

        public int DoRun()
        {
            mock.Protected().Arrange(f => f.ProtectedGetSomething()).Returns(1234);
            Assert.Equal(expected: 1234, mock.Object.InvokeProtectedGetSomething());

            mock.Object.InvokeProtectedDoSomething(5678);
            mock.Protected().Assert(f => f.ProtectedDoSomething(5678));

            mock.Protected().Arrange(f => f.ProtectedInternalGetSomething()).Returns(1234);
            Assert.Equal(expected: 1234, mock.Object.InvokeProtectedInternalGetSomething());

            mock.Object.InvokeProtectedInternalDoSomething(5678);
            mock.Protected().Assert(f => f.ProtectedInternalDoSomething(5678));

            return 42;
        }
    }
}
