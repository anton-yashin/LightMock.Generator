using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassThrowsExceptionOnRefStruct : ITestScript<AAbstractClassThrowsExceptionOnRefStruct>
    {
        private readonly Mock<AAbstractClassThrowsExceptionOnRefStruct> mock;

        public AbstractClassThrowsExceptionOnRefStruct()
            => mock = new Mock<AAbstractClassThrowsExceptionOnRefStruct>();

        public IMock<AAbstractClassThrowsExceptionOnRefStruct> Context => mock;

        public AAbstractClassThrowsExceptionOnRefStruct MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
