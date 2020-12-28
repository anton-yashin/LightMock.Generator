namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class GenericClassAndGenericBaseClass<T> : AGenericClassAndGenericBaseClass<T>
    {
    }

    public sealed class GenericClassAndGenericBaseClassTest
    {
        private readonly IP2P_AGenericClassAndGenericBaseClass<int> p2p;
        private readonly MockContext<IP2P_AGenericClassAndGenericBaseClass<int>> mockContext;

        public GenericClassAndGenericBaseClassTest(IP2P_AGenericClassAndGenericBaseClass<int> p2p, MockContext<IP2P_AGenericClassAndGenericBaseClass<int>> mockContext)
        {
            this.p2p = p2p;
            this.mockContext = mockContext;
        }

        public void TestProtectedMembers()
        {
            p2p.ProtectedDoSomething(1234);
            mockContext.Assert(f => f.ProtectedDoSomething(1234));

            mockContext.Arrange(f => f.ProtectedGetSomething()).Returns(5678);
            Xunit.Assert.Equal(5678, p2p.ProtectedGetSomething());

            mockContext.Arrange(f => f.ProtectedOnlyGet).Returns(9012);
            Xunit.Assert.Equal(9012, p2p.ProtectedOnlyGet);

            mockContext.ArrangeProperty(f => f.ProtectedGetAndSet);
            p2p.ProtectedGetAndSet = 3456;
            Xunit.Assert.Equal(3456, p2p.ProtectedGetAndSet);
        }
    }
}
