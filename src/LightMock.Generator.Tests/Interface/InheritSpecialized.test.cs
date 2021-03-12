using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public sealed class InheritSpecialized : ITestScript<IInheritSpecialized>
    {
        private readonly Mock<IInheritSpecialized> mock;

        public InheritSpecialized() => mock = new Mock<IInheritSpecialized>();

        public IMock<IInheritSpecialized> Context => mock;

        public IInheritSpecialized MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
