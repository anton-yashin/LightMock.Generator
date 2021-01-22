using System;

namespace LightMock.Generator.Tests.Mock
{
    public class InherittedInterface : ITestScript<IInherittedInterface>
    {
        private readonly Mock<IInherittedInterface> mock;

        public InherittedInterface() => mock = new Mock<IInherittedInterface>();

        public MockContext<IInherittedInterface> Context => mock;

        public IInherittedInterface MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
