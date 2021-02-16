using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Delegate
{
    public class NestedDelegateWithGenerics : ITestScript<XNestedInterface<int>.XContainer<long>.SomeDelegate<object>>
    {
        private readonly Mock<XNestedInterface<int>.XContainer<long>.SomeDelegate<object>> mock;

        public NestedDelegateWithGenerics()
            => mock = new Mock<XNestedInterface<int>.XContainer<long>.SomeDelegate<object>>();

        public IMock<XNestedInterface<int>.XContainer<long>.SomeDelegate<object>> Context => mock;

        public XNestedInterface<int>.XContainer<long>.SomeDelegate<object> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
