using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AssertSet_IsAny : ITestScript<AAssertSet_IsAny>
    {
        private readonly Mock<AAssertSet_IsAny> mock;

        public AssertSet_IsAny() => mock = new Mock<AAssertSet_IsAny>();

        public IMock<AAssertSet_IsAny> Context => mock;

        public AAssertSet_IsAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
