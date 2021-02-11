using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class InterfaceThrowsExceptionOnRefStruct : ITestScript<IInterfaceThrowsExceptionOnRefStruct>
    {
        private readonly Mock<IInterfaceThrowsExceptionOnRefStruct> mock;

        public InterfaceThrowsExceptionOnRefStruct()
            => mock = new Mock<IInterfaceThrowsExceptionOnRefStruct>();

        public IMock<IInterfaceThrowsExceptionOnRefStruct> Context => mock;

        public IInterfaceThrowsExceptionOnRefStruct MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
