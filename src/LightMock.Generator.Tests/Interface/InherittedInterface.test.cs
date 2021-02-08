using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    public class InherittedInterface : ITestScript<IInherittedInterface>
    {
        private readonly Mock<IInherittedInterface> mock;

        public InherittedInterface() => mock = new Mock<IInherittedInterface>();

        public IMock<IInherittedInterface> Context => mock;

        public IInherittedInterface MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
