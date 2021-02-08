using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class CantProcessSealedClass : ITestScript<SomeSealedClass>
    {
        private readonly Mock<SomeSealedClass> mock;

        public CantProcessSealedClass()
            => mock = new Mock<SomeSealedClass>();

        public IMock<SomeSealedClass> Context => mock;

        public SomeSealedClass MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
