using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class TypeCachingWithGenericAbstractClass : ITestScript<ATypeCachingWithGenericAbstractClass<int>>
    {
        private readonly Mock<ATypeCachingWithGenericAbstractClass<int>> mock;

        public TypeCachingWithGenericAbstractClass()
            => mock = new Mock<ATypeCachingWithGenericAbstractClass<int>>();

        public MockContext<ATypeCachingWithGenericAbstractClass<int>> Context => mock;

        public ATypeCachingWithGenericAbstractClass<int> MockObject => mock.Object;

        public int DoRun()
        {
            var another = new Mock<ATypeCachingWithGenericAbstractClass<int>>();
            var o = another.Object;
            Assert.NotNull(o);

            return 42;
        }
    }
}
