using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeAddRemove_When : ITestScript<IArrangeAddRemove_When>
    {
        private readonly Mock<IArrangeAddRemove_When> mock;

        public ArrangeAddRemove_When() => mock = new Mock<IArrangeAddRemove_When>();
        public IMock<IArrangeAddRemove_When> Context => mock;

        public IArrangeAddRemove_When MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
