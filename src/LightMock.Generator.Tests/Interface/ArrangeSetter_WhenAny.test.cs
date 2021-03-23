using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeSetter_WhenAny : ITestScript<IArrangeSetter_WhenAny>
    {
        private readonly Mock<IArrangeSetter_WhenAny> mock;

        public ArrangeSetter_WhenAny() => mock = new Mock<IArrangeSetter_WhenAny>();

        public IMock<IArrangeSetter_WhenAny> Context => mock;

        public IArrangeSetter_WhenAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
