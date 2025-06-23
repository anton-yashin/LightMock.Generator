using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Threading.Tasks;
using static LightMock.Generator.Tests.Issues.HintNames;

namespace LightMock.Generator.Tests.Issues
{
    public class HintNames : ITestScript<IHintNames>
    {
        public interface I1 { }

        public delegate TResult SomeDelegate<T1, T2, TResult>(T1 a1, T2 a2)
            where T1 : allows ref struct
            where T2 : I1, allows ref struct
            where TResult : new(), allows ref struct;


        private readonly Mock<IHintNames> mock;

        public HintNames()
            => mock = new Mock<IHintNames>();

        public IMock<IHintNames> Context => mock;

        public IHintNames MockObject => mock.Object;

        public int DoRun()
        {
            new Mock<Action>();
            new Mock<Action<object>>();
            new Mock<Func<object>>();
            new Mock<SomeDelegate<object, I1, object>>();
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
