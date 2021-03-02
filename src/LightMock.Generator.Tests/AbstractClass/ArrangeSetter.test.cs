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
            mock.ArrangeSetter(uidPart2: 271289, uidPart1: "c:\\some\\path\\to\\file", expression: f => f.Set = The<string>.Is(s => s == "4567"))
                .Throws(() => new ValidProgramException(nameof(AArrangeSetter.Set)));
            return 42;
        }
    }
}
