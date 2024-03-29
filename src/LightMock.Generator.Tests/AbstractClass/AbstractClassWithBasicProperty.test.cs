﻿using System;
using Xunit;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassWithBasicProperty : ITestScript<AAbstractClassWithBasicProperty>
    {
        private readonly Mock<AAbstractClassWithBasicProperty> mock;

        public AbstractClassWithBasicProperty()
            => mock = new Mock<AAbstractClassWithBasicProperty>();

        public IMock<AAbstractClassWithBasicProperty> Context => mock;

        public AAbstractClassWithBasicProperty MockObject => mock.Object;

        public int DoRun()
        {
            mock.Protected().Arrange(f => f.ProtectedOnlyGet).Returns(1234);
            Assert.Equal(1234, mock.Object.InvokeProtectedOnlyGet);

            mock.Protected().ArrangeProperty(f => f.ProtectedGetAndSet);
            mock.Object.InvokeProtectedGetAndSet = 5678;
            Assert.Equal(5678, mock.Object.InvokeProtectedGetAndSet);

            mock.Protected().Arrange(f => f.ProtectedInternalOnlyGet).Returns(1234);
            Assert.Equal(1234, mock.Object.InvokeProtectedInternalOnlyGet);

            mock.Protected().ArrangeProperty(f => f.ProtectedInternalGetAndSet);
            mock.Object.InvokeProtectedInternalGetAndSet = 5678;
            Assert.Equal(5678, mock.Object.InvokeProtectedInternalGetAndSet);

            return 42;
        }
    }
}
