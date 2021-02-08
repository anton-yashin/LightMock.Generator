using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class InheritAbstractClass : ITestScript<AInheritAbstractClass>
    {
        private readonly Mock<AInheritAbstractClass> mock;

        public InheritAbstractClass() => mock = new Mock<AInheritAbstractClass>();

        public IMock<AInheritAbstractClass> Context => mock;

        public AInheritAbstractClass MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
