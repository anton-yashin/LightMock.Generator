using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class TypeCachingWithAbstractClass : ITestScript<ATypeCachingWithAbstractClass>
    {
        private readonly Mock<ATypeCachingWithAbstractClass> mock;

        public TypeCachingWithAbstractClass()
            => mock = new Mock<ATypeCachingWithAbstractClass>();

        public MockContext<ATypeCachingWithAbstractClass> Context => mock;

        public ATypeCachingWithAbstractClass MockObject => mock.Object;

        public int DoRun()
        {
            var another = new Mock<ATypeCachingWithAbstractClass>();
            var o = another.Object;
            Assert.NotNull(o);

            return 42;
        }
    }
}
