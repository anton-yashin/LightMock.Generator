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

        public int TestProtectedMembers()
        {
            mockContext.Arrange(f => f.ProtectedGenericReturn<int>()).Returns(1234);
            Xunit.Assert.Equal(1234, p2p.ProtectedGenericReturn<int>());

            p2p.ProtectedGenericParam<int>(5678);
            mockContext.Assert(f => f.ProtectedGenericParam<int>(5678));

            var p = new object();
            p2p.ProtectedGenericWithClassConstraint(p);
            mockContext.Assert(f => f.ProtectedGenericWithClassConstraint(p));

            p2p.ProtectedGenericWithStructConstraint<int>(1234);
            mockContext.Assert(f => f.ProtectedGenericWithStructConstraint<int>(1234));

            p2p.ProtectedGenericWithConstraint(1234);
            mockContext.Assert(f => f.ProtectedGenericWithConstraint(1234));

            p2p.ProtectedGenericWithManyConstraints<object, int, long>(p, 123, 456);
            mockContext.Assert(f => f.ProtectedGenericWithManyConstraints<object, int, long>(p, 123, 456));

            return 42;
        }
    }
}
