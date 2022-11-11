using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class MethodWithInParameter : ITestScript<AMethodWithInParameter>
    {
        private readonly Mock<AMethodWithInParameter> mock;

        public MethodWithInParameter()
            => mock = new Mock<AMethodWithInParameter>();

        public IMock<AMethodWithInParameter> Context => mock;

        public AMethodWithInParameter MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
