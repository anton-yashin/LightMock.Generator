using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class MethodWithInParameter : ITestScript<IMethodWithInParameter>
    {
        private readonly Mock<IMethodWithInParameter> mock;

        public MethodWithInParameter()
            => mock = new Mock<IMethodWithInParameter>();

        public IMock<IMethodWithInParameter> Context => mock;

        public IMethodWithInParameter MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
