using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericAbstractClassWithGenericEvent : ITestScript<AGenericAbstractClassWithGenericEvent<int>>
    {
        private readonly Mock<AGenericAbstractClassWithGenericEvent<int>> mock;

        public GenericAbstractClassWithGenericEvent()
            => mock = new Mock<AGenericAbstractClassWithGenericEvent<int>>();

        public MockContext<AGenericAbstractClassWithGenericEvent<int>> Context => mock;

        public AGenericAbstractClassWithGenericEvent<int> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
