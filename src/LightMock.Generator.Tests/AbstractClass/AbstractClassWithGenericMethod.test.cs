using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassWithGenericMethod : ITestScript<AAbstractClassWithGenericMethod>
    {
        private readonly Mock<AAbstractClassWithGenericMethod> mock;

        public AbstractClassWithGenericMethod()
            => mock = new Mock<AAbstractClassWithGenericMethod>();

        public IMock<AAbstractClassWithGenericMethod> Context => mock;

        public AAbstractClassWithGenericMethod MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
