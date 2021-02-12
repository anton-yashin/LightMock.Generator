using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    class ObsoleteSupport : ITestScript<AObsoleteSupport>
    {
        private readonly Mock<AObsoleteSupport> mock;

        public ObsoleteSupport()
            => mock = new Mock<AObsoleteSupport>();

        public IMock<AObsoleteSupport> Context => mock;

        public AObsoleteSupport MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
