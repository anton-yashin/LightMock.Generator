using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class ReservedSymbols : ITestScript<IReservedSymbols>
    {
        private readonly Mock<IReservedSymbols> mock;

        public ReservedSymbols() => mock = new Mock<IReservedSymbols>();

        public IMock<IReservedSymbols> Context => mock;

        public IReservedSymbols MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
