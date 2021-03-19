using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeSetter_When : ITestScript<AArrangeSetter_When>
    {
        private readonly Mock<AArrangeSetter_When> mock;

        public ArrangeSetter_When() => mock = new Mock<AArrangeSetter_When>(new object(), 1234);

        public IMock<AArrangeSetter_When> Context => mock;

        public AArrangeSetter_When MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
