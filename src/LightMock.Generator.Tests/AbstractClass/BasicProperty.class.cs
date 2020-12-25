using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public sealed partial class BasicProperty : ABasicProperty
    {
    }

    public class BasicPropertyTest
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
            const int KExpected = 9218719;
            mockContext.ArrangeProperty(f => f.GetAndSet);
            p2p.GetAndSet = KExpected;
            Xunit.Assert.Equal(KExpected, p2p.GetAndSet);
        }
    }

}
