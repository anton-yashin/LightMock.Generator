using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeSetter_On : ITestScript<IArrangeSetter_On>
    {
        private readonly Mock<IArrangeSetter_On> mock;

        public ArrangeSetter_On() => mock = new Mock<IArrangeSetter_On>();

        public IMock<IArrangeSetter_On> Context => mock;

        public IArrangeSetter_On MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
