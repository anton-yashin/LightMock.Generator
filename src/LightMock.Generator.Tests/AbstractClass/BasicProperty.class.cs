using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public sealed partial class BasicProperty : ABasicProperty
    {
    }

    public class BasicPropertyTest : ITestScript
    {
        private readonly IP2P_ABasicProperty p2p;
        private readonly MockContext<IP2P_ABasicProperty> mockContext;

        public BasicPropertyTest(IP2P_ABasicProperty p2p, MockContext<IP2P_ABasicProperty> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
            const int KExpected1 = 1234;
            mockContext.Arrange(f => f.ProtectedOnlyGet).Returns(KExpected1);
            Xunit.Assert.Equal(expected: KExpected1, p2p.ProtectedOnlyGet);

            const int KExpected2 = 9218719;
            mockContext.ArrangeProperty(f => f.ProtectedGetAndSet);
            p2p.ProtectedGetAndSet = KExpected2;
            Xunit.Assert.Equal(KExpected2, p2p.ProtectedGetAndSet);
        }
    }

}
