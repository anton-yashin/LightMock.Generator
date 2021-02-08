﻿using System;

namespace LightMock.Generator.Tests.Mock
{
    public class BasicDelegate : ITestScript<EventHandler>
    {
        private readonly Mock<EventHandler> mock;

        public BasicDelegate()
            => mock = new Mock<EventHandler>();

        public IMock<EventHandler> Context => mock;

        public EventHandler MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
