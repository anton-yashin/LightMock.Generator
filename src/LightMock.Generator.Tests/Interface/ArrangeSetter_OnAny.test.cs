using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeSetter_OnAny : ITestScript<IArrangeSetter_OnAny>
    {
        private readonly Mock<IArrangeSetter_OnAny> mock;

        public ArrangeSetter_OnAny() => mock = new Mock<IArrangeSetter_OnAny>();

        public IMock<IArrangeSetter_OnAny> Context => mock;

        public IArrangeSetter_OnAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
