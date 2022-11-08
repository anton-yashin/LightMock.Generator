using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public sealed class MethodWithOutParameter : ITestScript<AMethodWithOutParameter>
    {
        private readonly Mock<AMethodWithOutParameter> mock;

        public MethodWithOutParameter()
            => mock = new Mock<AMethodWithOutParameter>();

        public IMock<AMethodWithOutParameter> Context => mock;

        public AMethodWithOutParameter MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
