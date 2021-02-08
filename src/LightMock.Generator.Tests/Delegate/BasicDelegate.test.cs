using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Delegate
{
    public class BasicDelegate : ITestScript<EventHandler>
    {
        private readonly Mock<EventHandler> mock;

        public BasicDelegate()
            => mock = new Mock<EventHandler>();

        public IMock<EventHandler> Context => mock;

        public EventHandler MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
