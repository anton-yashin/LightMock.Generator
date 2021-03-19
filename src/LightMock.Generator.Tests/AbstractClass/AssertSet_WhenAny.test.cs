using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AssertSet_WhenAny : ITestScript<AAssertSet_WhenAny>
    {
        private readonly Mock<AAssertSet_WhenAny> mock;

        public AssertSet_WhenAny() => mock = new Mock<AAssertSet_WhenAny>();

        public IMock<AAssertSet_WhenAny> Context => mock;

        public AAssertSet_WhenAny MockObject => mock.Object;

        public int DoRun()
        {
            mock.Protected().AssertSet_WhenAny(f => f.ProtectedGetAndSet = "");
            mock.Protected().AssertSet_WhenAny(f => f.ProtectedGetAndSet = "", Invoked.Once);
            mock.Protected().AssertSet_WhenAny(f => f.ProtectedSetOnly = "");
            mock.Protected().AssertSet_WhenAny(f => f.ProtectedSetOnly = "", Invoked.Once);

            return 42;
        }
    }
}
