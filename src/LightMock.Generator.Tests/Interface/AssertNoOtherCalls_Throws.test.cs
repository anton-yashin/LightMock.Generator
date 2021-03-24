using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class AssertNoOtherCalls_Throws : ITestScript<IAssertNoOtherCalls_Throws>
    {
        private readonly Mock<IAssertNoOtherCalls_Throws> mock;

        public AssertNoOtherCalls_Throws() => mock = new Mock<IAssertNoOtherCalls_Throws>();
        public IMock<IAssertNoOtherCalls_Throws> Context => mock;

        public IAssertNoOtherCalls_Throws MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
