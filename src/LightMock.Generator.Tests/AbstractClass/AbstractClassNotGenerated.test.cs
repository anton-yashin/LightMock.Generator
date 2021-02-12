using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassNotGenerated : ITestScript<AAbstractClassNotGenerated>
    {
        private Lazy<Mock<AAbstractClassNotGenerated>> lazyMock;

        public AbstractClassNotGenerated()
            => lazyMock = new Lazy<Mock<AAbstractClassNotGenerated>>(() => GetMock<AAbstractClassNotGenerated>());

        public IMock<AAbstractClassNotGenerated> Context => lazyMock.Value;

        public AAbstractClassNotGenerated MockObject => lazyMock.Value.Object;

        public int DoRun()
        {
            return 42;
        }

        static Mock<T> GetMock<T>()
            where T : class
        {
            return new Mock<T>();
        }
    }
}
