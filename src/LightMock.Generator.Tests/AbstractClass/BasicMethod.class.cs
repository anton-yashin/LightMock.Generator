namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class BasicMethod : ABasicMethod { }

    public class BasicMethodTest : ITestScript
    {
        private readonly IP2P_ABasicMethod p2p;
        private readonly MockContext<IP2P_ABasicMethod> mockContext;

        public BasicMethodTest(IP2P_ABasicMethod p2p, MockContext<IP2P_ABasicMethod> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
            mockContext.Arrange(f => f.ProtectedGetSomething()).Returns(1234);
            Xunit.Assert.Equal(expected: 1234, p2p.ProtectedGetSomething());

            p2p.ProtectedDoSomething(1234);
            mockContext.Assert(f => f.ProtectedDoSomething(1234));
        }
    }


}
