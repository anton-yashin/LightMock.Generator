using System;

namespace LightMock.Generator.Tests.Mock
{
    public class InheritAbstractClass : ITestScript<AInheritAbstractClass>
    {
        private readonly Mock<AInheritAbstractClass> mock;

        public InheritAbstractClass() => mock = new Mock<AInheritAbstractClass>();

        public MockContext<AInheritAbstractClass> Context => mock;

        public AInheritAbstractClass MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
