using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class MethodWithRefParameter : ITestScript<IMethodWithRefParameter>
    {
        private readonly Mock<IMethodWithRefParameter> mock;

        public MethodWithRefParameter()
            => mock = new Mock<IMethodWithRefParameter>();

        public IMock<IMethodWithRefParameter> Context => mock;

        public IMethodWithRefParameter MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
