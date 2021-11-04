using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AnalyzerOptions
{
    public class DisableCodeGeneration : ITestScript<IDisableCodeGeneration>
    {
        private readonly Mock<IDisableCodeGeneration> mock;

        public DisableCodeGeneration() => mock = new Mock<IDisableCodeGeneration>();

        public IMock<IDisableCodeGeneration> Context => mock;

        public IDisableCodeGeneration MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
