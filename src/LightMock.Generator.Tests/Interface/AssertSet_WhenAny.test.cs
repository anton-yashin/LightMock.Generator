using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class AssertSet_WhenAny : ITestScript<IAssertSet_WhenAny>
    {
        private readonly Mock<IAssertSet_WhenAny> mock;

        public AssertSet_WhenAny() => mock = new Mock<IAssertSet_WhenAny>();

        public IMock<IAssertSet_WhenAny> Context => mock;

        public IAssertSet_WhenAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
