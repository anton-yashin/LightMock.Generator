using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class TypeCachingWithInterface : ITestScript<ITypeCachingWithInterface>
    {
        private readonly Mock<ITypeCachingWithInterface> mock;

        public TypeCachingWithInterface()
            => mock = new Mock<ITypeCachingWithInterface>();

        public IMock<ITypeCachingWithInterface> Context => mock;

        public ITypeCachingWithInterface MockObject => mock.Object;

        public int DoRun()
        {
            var another = new Mock<ITypeCachingWithInterface>();
            var o = another.Object;
            Assert.NotNull(o);

            return 42;
        }
    }
}
