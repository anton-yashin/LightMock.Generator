using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    internal abstract class AGenerateForInternalClass
    {
    }

    internal sealed class GenerateForInternalClass : ITestScript<AGenerateForInternalClass>
    {
        private readonly Mock<AGenerateForInternalClass> mock;

        public GenerateForInternalClass()
        {
            mock = new Mock<AGenerateForInternalClass>();
        }

        public IMock<AGenerateForInternalClass> Context => mock;

        public AGenerateForInternalClass MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
