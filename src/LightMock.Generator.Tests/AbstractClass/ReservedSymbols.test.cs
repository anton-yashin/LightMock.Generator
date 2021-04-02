using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ReservedSymbols : ITestScript<AReservedSymbols>
    {
        private readonly Mock<AReservedSymbols> mock;

        public ReservedSymbols() => mock = new Mock<AReservedSymbols>();

        public IMock<AReservedSymbols> Context => mock;

        public AReservedSymbols MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
