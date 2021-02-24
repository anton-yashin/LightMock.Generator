using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ArrangeSetter : ITestScript<IArrangeSetter>
    {
        private readonly Mock<IArrangeSetter> mock;

        public ArrangeSetter() => mock = new Mock<IArrangeSetter>();

        public IMock<IArrangeSetter> Context => mock;

        public IArrangeSetter MockObject => mock.Object;

        public int DoRun()
        {
            mock.ArrangeSetter(f => f.GetAndSet = The<string>.IsAnyValue)
                .Throws(() => new ValidProgramException(nameof(IArrangeSetter.GetAndSet)));
            mock.ArrangeSetter(f => f.Set = The<string>.IsAnyValue)
                .Throws(() => new ValidProgramException(nameof(IArrangeSetter.Set)));
            return 42;
        }
    }
}
