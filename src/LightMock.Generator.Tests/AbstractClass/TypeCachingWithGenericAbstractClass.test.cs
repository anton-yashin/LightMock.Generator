using System;
using Xunit;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class TypeCachingWithGenericAbstractClass : ITestScript<ATypeCachingWithGenericAbstractClass<int>>
    {
        private readonly Mock<ATypeCachingWithGenericAbstractClass<int>> mock;

        public TypeCachingWithGenericAbstractClass()
            => mock = new Mock<ATypeCachingWithGenericAbstractClass<int>>();

        public IMock<ATypeCachingWithGenericAbstractClass<int>> Context => mock;

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
