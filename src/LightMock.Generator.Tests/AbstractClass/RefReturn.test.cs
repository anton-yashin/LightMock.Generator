using LightMock.Generator.Tests.TestAbstractions;
using System;
using Xunit;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class RefReturn : ITestScript<ARefReturn>
    {
        private readonly Mock<ARefReturn> mock;

        public RefReturn() => mock = new Mock<ARefReturn>();

        public IMock<ARefReturn> Context => mock;

        public ARefReturn MockObject => mock.Object;

        public int DoRun()
        {
            const string EXPECTED_STRING = nameof(EXPECTED_STRING);
            var expectedDateTime = DateTime.UtcNow;
            mock.RefReturn().Arrange(f => f.Foo()).Returns(() => EXPECTED_STRING);
            mock.RefReturn().Arrange(f => f.Bar()).Returns(() => expectedDateTime);

            mock.RefReturn().Assert(f => f.Foo(), Invoked.Never);
            mock.RefReturn().Assert(f => f.Bar(), Invoked.Never);

            Assert.Equal(EXPECTED_STRING, MockObject.Foo());
            Assert.Equal(expectedDateTime, MockObject.Bar());

            mock.RefReturn().Assert(f => f.Foo());
            mock.RefReturn().Assert(f => f.Bar());
            return 42;
        }
    }
}
