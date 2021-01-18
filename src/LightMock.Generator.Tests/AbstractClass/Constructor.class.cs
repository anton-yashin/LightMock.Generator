using System;
using System.Collections.Generic;
using Xunit;

namespace LightMock.Generator.Tests.AbstractClass
{
    [GenerateMock]
    public partial class Constructor : AConstructor { }

    public class ConstructorTest : ITestScript
    {
        public ConstructorTest()
        {
        }

        public void TestProtectedMembers()
        {
            var context = new MockContext<AConstructor>();
            var protectedContext = new MockContext<IP2P_AConstructor>();

            var expectedObject = new object();
            int expectedInt = 123;
            long expectedLong = 456;

            var mc1 = new Constructor(context, protectedContext, expectedInt, expectedLong, expectedObject);
            Assert.NotNull(mc1);
            Assert.Equal(expectedInt, mc1.P1);
            Assert.Equal(expectedLong, mc1.P2);
            Assert.Same(expectedObject, mc1.P3);
            Assert.Null(mc1.Objects);

            var mc2 = new Constructor(context, protectedContext);
            Assert.NotNull(mc2);
            Assert.Null(mc2.P1);
            Assert.Null(mc2.P2);
            Assert.Null(mc2.P3);
            Assert.Null(mc2.Objects);

            var expectedObjects = new object[0];
            var mc3 = new Constructor(context, protectedContext, expectedObjects);
            Assert.NotNull(mc3);
            Assert.Null(mc3.P1);
            Assert.Null(mc3.P2);
            Assert.Null(mc3.P3);
            Assert.Same(expectedObjects, mc3.Objects);
        }
    }
}
