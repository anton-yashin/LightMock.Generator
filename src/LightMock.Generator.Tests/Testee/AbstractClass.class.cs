namespace LightMock.Generator.Tests.Testee
{
    [GenerateMock]
    public partial class ConcreteClass : AbstractClass { }

    public class ConcreteClassTest
    {
        private readonly IP2P_AbstractClass p2p;
        private readonly MockContext<IP2P_AbstractClass> mockContext;

        public ConcreteClassTest(IP2P_AbstractClass p2p, MockContext<IP2P_AbstractClass> mockContext)
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
