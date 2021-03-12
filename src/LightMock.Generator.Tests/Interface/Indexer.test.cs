using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class Indexer : ITestScript<IIndexer<string>>
    {
        private readonly Mock<IIndexer<string>> mock;

        public Indexer() => mock = new Mock<IIndexer<string>>();

        public IMock<IIndexer<string>> Context => mock;

        public IIndexer<string> MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
