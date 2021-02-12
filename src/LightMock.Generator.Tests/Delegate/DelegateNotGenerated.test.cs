using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Delegate
{
    public class DelegateNotGenerated : ITestScript<EventHandler>
    {
        private readonly Lazy<Mock<EventHandler>> lazyMock;

        public DelegateNotGenerated()
            => lazyMock = new Lazy<Mock<EventHandler>>(() => GetMock<EventHandler>());

        public IMock<EventHandler> Context => lazyMock.Value;

        public EventHandler MockObject => lazyMock.Value.Object;

        public int DoRun()
        {
            return 42;
        }

        static Mock<T> GetMock<T>()
            where T : class
        {
            return new Mock<T>();
        }
    }
}
