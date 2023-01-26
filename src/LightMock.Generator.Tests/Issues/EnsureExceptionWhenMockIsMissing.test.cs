using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Issues
{
    public class EnsureExceptionWhenMockIsMissing : ITestScript<IEnsureExceptionWhenMockIsMissing>
    {
        private readonly Mock<IEnsureExceptionWhenMockIsMissing> mock;

        public EnsureExceptionWhenMockIsMissing() => mock = new Mock<IEnsureExceptionWhenMockIsMissing>();

        public IMock<IEnsureExceptionWhenMockIsMissing> Context => mock;

        public IEnsureExceptionWhenMockIsMissing MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
