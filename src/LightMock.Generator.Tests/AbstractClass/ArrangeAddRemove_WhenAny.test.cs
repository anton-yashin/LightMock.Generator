using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeAddRemove_WhenAny : ITestScript<AArrangeAddRemove_WhenAny>
    {
        private readonly Mock<AArrangeAddRemove_WhenAny> mock;

        public ArrangeAddRemove_WhenAny() => mock = new Mock<AArrangeAddRemove_WhenAny>();

        public IMock<AArrangeAddRemove_WhenAny> Context => mock;

        public AArrangeAddRemove_WhenAny MockObject => mock.Object;

        public int DoRun()
        {
            mock.Protected().ArrangeAdd_WhenAny(f => f.ProtectedEventHandler += null).Throws<ValidProgramException>();
            mock.Protected().ArrangeRemove_WhenAny(f => f.ProtectedEventHandler -= null).Throws<ValidProgramException>();
            return 42;
        }
    }
}
