using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class Indexer : ITestScript<AIndexer<string>>
    {
        private readonly Mock<AIndexer<string>> mock;

        public Indexer() => mock = new Mock<AIndexer<string>>();

        public IMock<AIndexer<string>> Context => mock;

        public AIndexer<string> MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
