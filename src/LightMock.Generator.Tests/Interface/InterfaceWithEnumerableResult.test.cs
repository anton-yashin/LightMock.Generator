using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Interface
{
    public class InterfaceWithEnumerableResult : ITestScript<IInterfaceWithEnumerableResult>
    {
        private readonly Mock<IInterfaceWithEnumerableResult> mock;

        public InterfaceWithEnumerableResult()
            => mock = new Mock<IInterfaceWithEnumerableResult>();

        public IMock<IInterfaceWithEnumerableResult> Context => mock;

        public IInterfaceWithEnumerableResult MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
