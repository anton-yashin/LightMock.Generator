using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeSetter : ITestScript<AArrangeSetter>
    {
        private readonly Mock<AArrangeSetter> mock;

        public ArrangeSetter()
            => mock = new Mock<AArrangeSetter>();

        public IMock<AArrangeSetter> Context => mock;

        public AArrangeSetter MockObject => mock.Object;

        public int DoRun()
        {
            mock.ArrangeSetter(f => f.GetAndSet = The<string>.Is(s => s == "1234"))
                .Throws(() => new ValidProgramException(nameof(AArrangeSetter.GetAndSet)));
            mock.ArrangeSetter(uidPart2: 271289, uidPart1: "c:\\some\\path\\to\\file", expression: f => f.SetOnly = The<string>.Is(s => s == "4567"))
                .Throws(() => new ValidProgramException(nameof(AArrangeSetter.SetOnly)));
            mock.Protected().ArrangeSetter(f => f.ProtectedGetAndSet = The<string>.Is(s => s == "8901"))
                .Throws(() => new ValidProgramException("ProtectedGetAndSet"));
            mock.Protected().ArrangeSetter(uidPart1: "c:\\some\\path\\to\\file", uidPart2: 819273, expression: f => f.ProtectedSetOnly = The<string>.Is(s => s == "2345"))
                .Throws(() => new ValidProgramException("ProtectedSetOnly"));
            return 42;
        }
    }
}
