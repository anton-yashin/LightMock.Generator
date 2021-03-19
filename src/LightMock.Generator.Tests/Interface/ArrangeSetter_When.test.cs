using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeSetter_When : ITestScript<IArrangeSetter_When>
    {
        private readonly Mock<IArrangeSetter_When> mock;

        public ArrangeSetter_When() => mock = new Mock<IArrangeSetter_When>();

        public IMock<IArrangeSetter_When> Context => mock;

        public IArrangeSetter_When MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
