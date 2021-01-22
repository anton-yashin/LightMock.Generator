using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class InheritAbstractClass : AInheritAbstractClass
    {
    }

    public class InheritAbstractClassTest : ITestScript
    {
        private readonly IP2P_AInheritAbstractClass p2p;
        private readonly MockContext<IP2P_AInheritAbstractClass> mockContext;

        public InheritAbstractClassTest(IP2P_AInheritAbstractClass p2p, MockContext<IP2P_AInheritAbstractClass> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public int TestProtectedMembers()
        {
            mockContext.Assert(f => f.ProtectedFoo());
            mockContext.Assert(f => f.ProtectedBar());
            mockContext.Assert(f => f.ProtectedBaz());

            return 42;
        }
    }

}
