namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class ConcreteClass : BasicMethod { }

    public class ConcreteClassTest
    {
        private readonly IP2P_BasicMethod p2p;
        private readonly MockContext<IP2P_BasicMethod> mockContext;

        public ConcreteClassTest(IP2P_BasicMethod p2p, MockContext<IP2P_BasicMethod> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
            p2p.DoSomething(1234);

            mockContext.Assert(f => f.DoSomething(1234));
        }
    }


}
