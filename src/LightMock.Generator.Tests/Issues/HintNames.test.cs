using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Issues
{
    public class HintNames : ITestScript<IHintNames>
    {
        private readonly Mock<IHintNames> mock;

        public HintNames()
            => mock = new Mock<IHintNames>();

        public IMock<IHintNames> Context => mock;

        public IHintNames MockObject => mock.Object;

        public int DoRun()
        {
            new Mock<Action>();
            new Mock<Func<object>>();
            new Mock<Func<Task>>();
            new Mock<Func<Task<object>>>();
            new Mock<IHintNames<object>>();
            new Mock<IHintNames<Task>>();
            new Mock<IHintNames<Task<object>>>();
            new Mock<AHintNames>();
            new Mock<AHintNames<object>>();
            new Mock<AHintNames<Task>>();
            new Mock<AHintNames<Task<object>>>();
            return 42;
        }
    }
}
