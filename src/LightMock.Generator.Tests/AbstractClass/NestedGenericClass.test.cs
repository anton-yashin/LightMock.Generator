using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class NestedGenericClass : ITestScript<ANestedGenericClass<int>.AContainingClass<long>.ATest<AFoo>>
    {
        private readonly Mock<ANestedGenericClass<int>.AContainingClass<long>.ATest<AFoo>> mock;

        public NestedGenericClass()
            => mock = new Mock<ANestedGenericClass<int>.AContainingClass<long>.ATest<AFoo>>();

        public IMock<ANestedGenericClass<int>.AContainingClass<long>.ATest<AFoo>> Context => mock;

        public ANestedGenericClass<int>.AContainingClass<long>.ATest<AFoo> MockObject => mock.Object;

        public int DoRun()
        {
            throw new NotImplementedException();
        }
    }
}
