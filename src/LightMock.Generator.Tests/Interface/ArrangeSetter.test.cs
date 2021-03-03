using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeSetter : ITestScript<IArrangeSetter>
    {
        private readonly IMock<IArrangeSetter> mock;

        public ArrangeSetter() => mock = new Mock<IArrangeSetter>();

        public IMock<IArrangeSetter> Context => mock;

        public IArrangeSetter MockObject => mock.Object;

        public int DoRun()
        {
            mock // check for 
                .ArrangeSetter( // strange invocation
                    f => f.GetAndSet = The<string>.Is(s => s == "1234"))
                .Throws(() => new ValidProgramException(nameof(IArrangeSetter.GetAndSet)));
            mock.ArrangeSetter(uidPart2: 271289, uidPart1: "c:\\some\\path\\to\\file", expression: f => f.Set = The<string>.Is(s => s == "4567"))
                .Throws(() => new ValidProgramException(nameof(IArrangeSetter.Set)));
            return 42;
        }
    }
}
