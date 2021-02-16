using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class DontOverrideSupport : ITestScript<ADontOverrideSupport>
    {
        private readonly Mock<ADontOverrideSupport> mock;

        public DontOverrideSupport()
            => mock = new Mock<ADontOverrideSupport>();

        public IMock<ADontOverrideSupport> Context => mock;

        public ADontOverrideSupport MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
