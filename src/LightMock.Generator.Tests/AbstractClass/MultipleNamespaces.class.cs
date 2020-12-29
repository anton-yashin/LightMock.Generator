using System;
using LightMock.Generator.Tests.AbstractClass.Namespace2;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class MultipleNamespaces : AMultipleNamespaces
    {
    }
}


namespace LightMock.Generator.Tests.AbstractClass
{

    using LightMock.Generator.Tests.AbstractClass.Namespace2;
    using LightMock.Generator.Tests.AbstractClass.Namespace1;

    public sealed class MultipleNamespacesTest : ITestScript
    {
        private readonly IP2P_AMultipleNamespaces p2p;
        private readonly MockContext<IP2P_AMultipleNamespaces> mockContext;

        public MultipleNamespacesTest(IP2P_AMultipleNamespaces p2p, MockContext<IP2P_AMultipleNamespaces> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
            var a1 = new AMultipleNamespacesArgument();
            p2p.ProtectedDoSomething(a1);
            mockContext.Assert(f => f.ProtectedDoSomething(a1));

            var a2 = new AMultipleNamespacesArgument();
            mockContext.Arrange(f => f.ProtectedGetSomething()).Returns(a2);
            Xunit.Assert.Same(a2, p2p.ProtectedGetSomething());

            var a3 = new AMultipleNamespacesArgument();
            mockContext.ArrangeProperty(f => f.ProtectedSomeProperty);
            p2p.ProtectedSomeProperty = a3;
            Xunit.Assert.Same(a3, p2p.ProtectedSomeProperty);

        }
    }
}
