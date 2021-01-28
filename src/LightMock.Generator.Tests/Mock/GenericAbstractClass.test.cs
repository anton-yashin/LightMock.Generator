using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericAbstractClass : ITestScript<AGenericAbstractClass<int>>
    {
        private readonly Mock<AGenericAbstractClass<int>> mock;

        public GenericAbstractClass()
            => mock = new Mock<AGenericAbstractClass<int>>();

        public IMock<AGenericAbstractClass<int>> Context => mock;

        public AGenericAbstractClass<int> MockObject => mock.Object;

        public int DoRun()
        {
            mock.Object.InvokeProtectedDoSomething(1234);
            mock.Protected().Assert(f => f.ProtectedDoSomething(1234));

            mock.Protected().Arrange(f => f.ProtectedGetSomething()).Returns(5678);
            Assert.Equal(5678, mock.Object.InvokeProtectedGetSomething());

            mock.Protected().Arrange(f => f.ProtectedOnlyGet).Returns(9012);
            Assert.Equal(9012, mock.Object.InvokeProtectedOnlyGet);

            mock.Protected().ArrangeProperty(f => f.ProtectedGetAndSet);
            mock.Object.InvokeProtectedGetAndSet = 3456;
            Assert.Equal(3456, mock.Object.InvokeProtectedGetAndSet);

            return 42;
        }
    }
}
