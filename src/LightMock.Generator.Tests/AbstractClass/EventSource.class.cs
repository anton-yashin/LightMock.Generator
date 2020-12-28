namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class EventSource : AEventSource
    { }

    public class EventSourceTest
    {
        private readonly EventSource p2p;
        private readonly MockContext<IP2P_AEventSource> mockContext;

        public EventSourceTest(EventSource p2p, MockContext<IP2P_AEventSource> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
        }
    }

}
