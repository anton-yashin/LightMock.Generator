using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeSetter_On : ITestScript<AArrangeSetter_On>
    {
        private readonly Mock<AArrangeSetter_On> mock;

        public ArrangeSetter_On() => mock = new Mock<AArrangeSetter_On>();

        public IMock<AArrangeSetter_On> Context => mock;

        public AArrangeSetter_On MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
