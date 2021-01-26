using System;

namespace LightMock.Generator.Tests.Mock
{
    public class CantProcessSealedClass : ITestScript<SomeSealedClass>
    {
        private readonly Mock<SomeSealedClass> mock;

        public CantProcessSealedClass()
            => mock = new Mock<SomeSealedClass>();

        public MockContext<SomeSealedClass> Context => mock;

        public SomeSealedClass MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
