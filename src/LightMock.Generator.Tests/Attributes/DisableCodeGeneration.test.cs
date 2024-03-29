﻿using LightMock.Generator.Tests.TestAbstractions;
using LightMock.Generator;
using System;

[assembly: DisableCodeGeneration]

namespace LightMock.Generator.Tests.Attributes
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
