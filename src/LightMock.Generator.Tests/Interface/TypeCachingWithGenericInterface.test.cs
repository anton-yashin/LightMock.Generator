using System;
using Xunit;

namespace LightMock.Generator.Tests.Interface
{
    public class TypeCachingWithGenericInterface : ITestScript<ITypeCachingWithGenericInterface<int>>
    {
        private readonly Mock<ITypeCachingWithGenericInterface<int>> mock;

        public TypeCachingWithGenericInterface()
            => mock = new Mock<ITypeCachingWithGenericInterface<int>>();

        public IMock<ITypeCachingWithGenericInterface<int>> Context => mock;

        public ITypeCachingWithGenericInterface<int> MockObject => mock.Object;

        public int DoRun()
        {
            var another = new Mock<ITypeCachingWithGenericInterface<int>>();
            var o = another.Object;
            Assert.NotNull(o);

            return 42;
        }
    }
}
