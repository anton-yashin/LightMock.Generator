using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class AssertSet_IsAny : ITestScript<IAssertSet_IsAny>
    {
        private readonly Mock<IAssertSet_IsAny> mock;

        public AssertSet_IsAny() => mock = new Mock<IAssertSet_IsAny>();

        public IMock<IAssertSet_IsAny> Context => mock;

        public IAssertSet_IsAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
