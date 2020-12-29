using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class GenericMethod : AGenericMethod
    {
    }

    public class GenericMethodTest : ITestScript
    {
        private readonly IP2P_AGenericMethod p2p;
        private readonly MockContext<IP2P_AGenericMethod> mockContext;

        public GenericMethodTest(IP2P_AGenericMethod p2p, MockContext<IP2P_AGenericMethod> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
            mockContext.Arrange(f => f.ProtectedGenericReturn<int>()).Returns(1234);
            Xunit.Assert.Equal(1234, p2p.ProtectedGenericReturn<int>());

            p2p.ProtectedGenericParam<int>(5678);
            mockContext.Assert(f => f.ProtectedGenericParam<int>(5678));

            var p = new object();
            p2p.ProtectedGenericWithConstraint(p);
            mockContext.Assert(f => f.ProtectedGenericWithConstraint(p));
        }
    }
}
