using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Collections.Generic;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeSetter_WhenAny : ITestScript<AArrangeSetter_WhenAny>
    {
        private readonly Mock<AArrangeSetter_WhenAny> mock;

        public ArrangeSetter_WhenAny() => mock = new Mock<AArrangeSetter_WhenAny>(new object(), 1234, (IEnumerable<object>)null!);

        public IMock<AArrangeSetter_WhenAny> Context => mock;

        public AArrangeSetter_WhenAny MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
