using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class InterfaceNotGenerated : ITestScript<IInterfaceNotGenerated>
    {

        private readonly Lazy<Mock<IInterfaceNotGenerated>> lazyMock;

        public InterfaceNotGenerated()
            => lazyMock = new Lazy<Mock<IInterfaceNotGenerated>>(() => GetMock<IInterfaceNotGenerated>());

        public IMock<IInterfaceNotGenerated> Context => lazyMock.Value;

        public IInterfaceNotGenerated MockObject => lazyMock.Value.Object;

        public int DoRun()
        {
            return 42;
        }

        static Mock<T> GetMock<T>()
            where T : class
        {
            return new Mock<T>();
        }
    }
}
