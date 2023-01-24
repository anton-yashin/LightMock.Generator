using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Issues
{
    public class TypeKeyAttributeRemoved : ITestScript<ITypeKeyAttributeRemoved>
    {
        private readonly Mock<ITypeKeyAttributeRemoved> mock;

        public TypeKeyAttributeRemoved() => mock = new Mock<ITypeKeyAttributeRemoved>();

        public IMock<ITypeKeyAttributeRemoved> Context => mock;

        public ITypeKeyAttributeRemoved MockObject => mock.Object;

        public int DoRun()
        {
            new Mock<ITypeKeyAttributeRemoved>();
            new Mock<ITypeKeyAttributeRemoved<object>>();
            new Mock<ATypeKeyAttributeRemoved>();
            new Mock<ATypeKeyAttributeRemoved<object>>();
            return 42;
        }
    }
}
