using LightMock.Generator.Tests.AbstractClass.Namespace4;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class EventSourceMultipleNamespaces : AEventSourceMultipleNamespaces
    {
    }

    public class EventSourceMultipleNamespacesTest : ITestScript
    {
        private readonly IP2P_AEventSourceMultipleNamespaces p2p;
        private readonly MockContext<IP2P_AEventSourceMultipleNamespaces> mockContext;

        public EventSourceMultipleNamespacesTest(IP2P_AEventSourceMultipleNamespaces p2p, MockContext<IP2P_AEventSourceMultipleNamespaces> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public int TestProtectedMembers()
        {
            return 42;
        }
    }

}
