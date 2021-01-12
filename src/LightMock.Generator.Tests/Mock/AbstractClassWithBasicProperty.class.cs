﻿using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    public class AbstractClassWithBasicProperty : ITestScript<AAbstractClassWithBasicProperty>
    {
        private readonly Mock<AAbstractClassWithBasicProperty> mock;

        public AbstractClassWithBasicProperty()
            => mock = new Mock<AAbstractClassWithBasicProperty>();

        public MockContext<AAbstractClassWithBasicProperty> Context => mock;

        public AAbstractClassWithBasicProperty MockObject => mock.Object;

        public int DoRun()
        {
            mock.ProtectedArrange(f => f.ProtectedOnlyGet).Returns(1234);
            Assert.Equal(1234, mock.Object.InvokeProtectedOnlyGet);

            mock.ProtectedArrangeProperty(f => f.ProtectedGetAndSet);
            mock.Object.InvokeProtectedGetAndSet = 5678;
            Assert.Equal(5678, mock.Object.InvokeProtectedGetAndSet);

            return 42;
        }
    }
}
