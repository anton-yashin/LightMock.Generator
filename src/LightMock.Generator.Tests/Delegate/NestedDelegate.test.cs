using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Delegate
{
    public class NestedDelegate : ITestScript<XNestedDelegate.SomeDelegate>
    {
        private readonly Mock<XNestedDelegate.SomeDelegate> mock;

        public NestedDelegate()
            => mock = new Mock<XNestedDelegate.SomeDelegate>();

        public IMock<XNestedDelegate.SomeDelegate> Context => mock;

        public XNestedDelegate.SomeDelegate MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
