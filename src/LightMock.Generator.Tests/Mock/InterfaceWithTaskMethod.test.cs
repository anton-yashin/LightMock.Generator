using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Mock
{
    public class InterfaceWithTaskMethod : ITestScript<IInterfaceWithTaskMethod>
    {
        private readonly Mock<IInterfaceWithTaskMethod> mock;

        public InterfaceWithTaskMethod()
            => mock = new Mock<IInterfaceWithTaskMethod>();

        public IMock<IInterfaceWithTaskMethod> Context => mock;

        public IInterfaceWithTaskMethod MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
