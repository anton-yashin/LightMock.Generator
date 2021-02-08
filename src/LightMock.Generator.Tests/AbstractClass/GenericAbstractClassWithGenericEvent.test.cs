using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class GenericAbstractClassWithGenericEvent : ITestScript<AGenericAbstractClassWithGenericEvent<int>>
    {
        private readonly Mock<AGenericAbstractClassWithGenericEvent<int>> mock;

        public GenericAbstractClassWithGenericEvent()
            => mock = new Mock<AGenericAbstractClassWithGenericEvent<int>>();

        public IMock<AGenericAbstractClassWithGenericEvent<int>> Context => mock;

        public AGenericAbstractClassWithGenericEvent<int> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
