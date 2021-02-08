using LightMock;
using System;
using Xunit;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    using Namespace1;
    using Namespace2;

    public class AbstractClassWithMultipleNamespaces : ITestScript<AAbstractClassWithMultipleNamespaces>
    {
        private readonly Mock<AAbstractClassWithMultipleNamespaces> mock;

        public AbstractClassWithMultipleNamespaces()
            => mock = new Mock<AAbstractClassWithMultipleNamespaces>();

        public IMock<AAbstractClassWithMultipleNamespaces> Context => mock;

        public AAbstractClassWithMultipleNamespaces MockObject => mock.Object;

        public int DoRun()
        {
            var arg1 = new MultipleNamespacesArgument();
            mock.Object.InvokeProtectedDoSomething(arg1);
            mock.Protected().Assert(f => f.ProtectedDoSomething(arg1));

            var arg2 = new MultipleNamespacesArgument();
            mock.Protected().Arrange(f => f.ProtectedGetSomething()).Returns(arg2);
            Assert.Same(expected: arg2, mock.Object.InvokeProtectedGetSomething());

            var arg3 = new MultipleNamespacesArgument();
            mock.Protected().ArrangeProperty(f => f.ProtectedSomeProperty);
            mock.Object.InvokeProtectedSomeProperty = arg3;
            Assert.Same(expected: arg3, mock.Object.InvokeProtectedSomeProperty);

            return 42;
        }
    }
}
