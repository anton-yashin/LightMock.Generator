namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class EventSourceGenericClass<T> : AEventSourceGenericClass<T>
    {
    }

    public class EventSourceGenericClassTest : ITestScript
    {
        private readonly IP2P_AEventSourceGenericClass<int> p2p;
        private readonly MockContext<IP2P_AEventSourceGenericClass<int>> mockContext;

        public EventSourceGenericClassTest(IP2P_AEventSourceGenericClass<int> p2p, MockContext<IP2P_AEventSourceGenericClass<int>> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
        }
    }

}
