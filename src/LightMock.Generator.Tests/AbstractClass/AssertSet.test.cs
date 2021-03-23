using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AssertSet : ITestScript<AAssertSet>
    {
        private readonly Mock<AAssertSet> mock;

        public AssertSet() => mock = new Mock<AAssertSet>();

        public IMock<AAssertSet> Context => mock;

        public AAssertSet MockObject => mock.Object;

        public int DoRun()
        {
            mock.Protected().AssertSet(f => f.ProtectedGetAndSet = The<string>.Is(s => s.Length > 5), Invoked.Once);
            mock.Protected().AssertSet(f => f.ProtectedSetOnly = The<string>.Is(s => s.StartsWith("hello")));
            mock.AssertSet(f => f.GetAndSet = The<string>.Is(s => s.Length > 5), Invoked.Once);
            mock.AssertSet(f => f.SetOnly = The<string>.Is(s => s.StartsWith("hello")));
            return 42;
        }
    }
}
