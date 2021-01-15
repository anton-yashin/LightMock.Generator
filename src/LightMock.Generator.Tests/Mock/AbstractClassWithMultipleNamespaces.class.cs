﻿using LightMock;
using System;
using Xunit;

namespace LightMock.Generator.Tests.Mock
{
    using Namespace1;
    using Namespace2;

    public class AbstractClassWithMultipleNamespaces : ITestScript<AAbstractClassWithMultipleNamespaces>
    {
        private readonly Mock<AAbstractClassWithMultipleNamespaces> mock;

        public AbstractClassWithMultipleNamespaces()
            => mock = new Mock<AAbstractClassWithMultipleNamespaces>();

        public MockContext<AAbstractClassWithMultipleNamespaces> Context => mock;

        public AAbstractClassWithMultipleNamespaces MockObject => mock.Object;

        public int DoRun()
        {
            var arg1 = new MultipleNamespacesArgument();
            mock.Object.InvokeProtectedDoSomething(arg1);
            mock.ProtectedAssert(f => f.ProtectedDoSomething(arg1));

            var arg2 = new MultipleNamespacesArgument();
            mock.ProtectedArrange(f => f.ProtectedGetSomething()).Returns(arg2);
            Assert.Same(expected: arg2, mock.Object.InvokeProtectedGetSomething());

            var arg3 = new MultipleNamespacesArgument();
            mock.ProtectedArrangeProperty(f => f.ProtectedSomeProperty);
            mock.Object.InvokeProtectedSomeProperty = arg3;
            Assert.Same(expected: arg3, mock.Object.InvokeProtectedSomeProperty);

            return 42;
        }
    }
}