using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    public class InterfaceWithGenericMethod : ITestScript<IInterfaceWithGenericMethod>
    {
        private readonly Mock<IInterfaceWithGenericMethod> mock;

        public InterfaceWithGenericMethod()
        {
            mock = new Mock<IInterfaceWithGenericMethod>();
        }

        public IMock<IInterfaceWithGenericMethod> Context => mock;

        public IInterfaceWithGenericMethod MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
