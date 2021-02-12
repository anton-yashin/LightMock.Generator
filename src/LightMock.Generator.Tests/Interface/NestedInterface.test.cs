using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class NestedInterface : ITestScript<INestedInterface.ITest>
    {
        private readonly Mock<INestedInterface.ITest> mock;

        public NestedInterface()
            => mock = new Mock<INestedInterface.ITest>();

        public IMock<INestedInterface.ITest> Context => mock;

        public INestedInterface.ITest MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
