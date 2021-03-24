using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class AssertNoOtherCalls : ITestScript<IAssertNoOtherCalls>
    {
        private readonly Mock<IAssertNoOtherCalls> mock;

        public AssertNoOtherCalls() => mock = new Mock<IAssertNoOtherCalls>();

        public IMock<IAssertNoOtherCalls> Context => mock;

        public IAssertNoOtherCalls MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
