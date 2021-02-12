using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class NestedClass : ITestScript<XNestedClass.ATest>
    {
        private readonly Mock<XNestedClass.ATest> mock;

        public NestedClass()
            => mock = new Mock<XNestedClass.ATest>();

        public IMock<XNestedClass.ATest> Context => mock;

        public XNestedClass.ATest MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
