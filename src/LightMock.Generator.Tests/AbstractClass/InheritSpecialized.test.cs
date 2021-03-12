using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public sealed class InheritSpecialized : ITestScript<AInheritSpecialized>
    {
        private readonly Mock<AInheritSpecialized> mock;

        public InheritSpecialized() => mock = new Mock<AInheritSpecialized>();

        public IMock<AInheritSpecialized> Context => mock;

        public AInheritSpecialized MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
