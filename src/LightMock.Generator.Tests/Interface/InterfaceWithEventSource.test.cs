﻿using System;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.Interface
{
    public class InterfaceWithEventSource : ITestScript<IInterfaceWithEventSource>
    {
        private readonly Mock<IInterfaceWithEventSource> mock;

        public InterfaceWithEventSource()
        {
            mock = new Mock<IInterfaceWithEventSource>();
        }

        public IMock<IInterfaceWithEventSource> Context => mock;

        public IInterfaceWithEventSource MockObject => mock.Object;

        public int DoRun() => 42;
    }
}
