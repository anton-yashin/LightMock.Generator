using System;
using System.Threading.Tasks;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    public class MethodWithOutParameter : ITestScript<IMethodWithOutParameter>
    {
        private readonly Mock<IMethodWithOutParameter> mock;

        public MethodWithOutParameter()
            => mock = new Mock<IMethodWithOutParameter>();

        public IMock<IMethodWithOutParameter> Context => mock;

        public IMethodWithOutParameter MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
