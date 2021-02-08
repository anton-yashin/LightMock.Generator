using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericDelegate : ITestScript<SomeGenericDelegate<object, int, long, long>>
    {
        private readonly Mock<SomeGenericDelegate<object, int, long, long>> mock;

        public GenericDelegate()
            => mock = new Mock<SomeGenericDelegate<object, int, long, long>>();

        public IMock<SomeGenericDelegate<object, int, long, long>> Context => mock;

        public SomeGenericDelegate<object, int, long, long> MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
