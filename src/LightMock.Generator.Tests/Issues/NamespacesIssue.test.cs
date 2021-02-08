using LightMock;
using LightMock.Generator;
using LightMock.Generator.Tests;
using System;
using Xunit;

namespace Playground
{
    public class NamespacesIssue : ITestScript<IFoo>
    {
        private readonly Mock<IFoo> mock;

        public NamespacesIssue() => mock = new Mock<IFoo>();

        public IMock<IFoo> Context => mock;

        public IFoo MockObject => mock.Object;

        public int DoRun()
        {
            var mock1 = new Mock<IFoo<int>>();
            var o1 = mock1.Object;
            Assert.NotNull(o1);

            var mock2 = new Mock<AFoo>();
            var o2 = mock2.Object;
            Assert.NotNull(o2);

            var mock3 = new Mock<AFoo<int>>();
            var o3 = mock3.Object;
            Assert.NotNull(o3);

            return 42;
        }
    }
}

namespace LightMock.Generator.Playground
{
    public static class Foo
    {
        public static void Bar() { }
    }
}
