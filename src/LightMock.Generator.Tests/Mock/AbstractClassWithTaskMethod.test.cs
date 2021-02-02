using System;

namespace LightMock.Generator.Tests.Mock
{
    public class AbstractClassWithTaskMethod : ITestScript<AAbstractClassWithTaskMethod>
    {
        private readonly Mock<AAbstractClassWithTaskMethod> mock;

        public AbstractClassWithTaskMethod()
            => mock = new Mock<AAbstractClassWithTaskMethod>();

        public IMock<AAbstractClassWithTaskMethod> Context => mock;

        public AAbstractClassWithTaskMethod MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
