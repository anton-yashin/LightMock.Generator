using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class MethodWithRefParameter : ITestScript<AMethodWithRefParameter>
    {
        private readonly Mock<AMethodWithRefParameter> mock;

        public MethodWithRefParameter()
            => mock = new Mock<AMethodWithRefParameter>();

        public IMock<AMethodWithRefParameter> Context => mock;

        public AMethodWithRefParameter MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
