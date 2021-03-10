using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeSetter_OnAny : ITestScript<AArrangeSetter_OnAny>
    {
        private readonly Mock<AArrangeSetter_OnAny> mock;

        public ArrangeSetter_OnAny() => mock = new Mock<AArrangeSetter_OnAny>();

        public IMock<AArrangeSetter_OnAny> Context => mock;

        public AArrangeSetter_OnAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
