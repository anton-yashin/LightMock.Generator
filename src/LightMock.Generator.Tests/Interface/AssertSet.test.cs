using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class AssertSet : ITestScript<IAssertSet>
    {
        private readonly Mock<IAssertSet> mock;

        public AssertSet() => mock = new Mock<IAssertSet>();

        public IMock<IAssertSet> Context => mock;

        public IAssertSet MockObject => mock.Object;

        public int DoRun()
        {
            mock.AssertSet(f => f.GetAndSet = The<string>.Is(s => s.Length > 5), Invoked.Once);
            mock.AssertSet(f => f.SetOnly = The<string>.Is(s => s.StartsWith("hello")));
            return 42;
        }
    }
}
