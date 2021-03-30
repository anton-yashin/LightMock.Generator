using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeAddRemove_WhenAny : ITestScript<IArrangeAddRemove_WhenAny>
    {
        private readonly Mock<IArrangeAddRemove_WhenAny> mock;

        public ArrangeAddRemove_WhenAny() => mock = new Mock<IArrangeAddRemove_WhenAny>();

        public IMock<IArrangeAddRemove_WhenAny> Context => mock;

        public IArrangeAddRemove_WhenAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
