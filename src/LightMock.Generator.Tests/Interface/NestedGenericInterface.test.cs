using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public class NestedGenericInterface : ITestScript<INestedGenericInterface<int>.IContainingInterface<long>.ITest<IBar>>
    {
        private readonly Mock<INestedGenericInterface<int>.IContainingInterface<long>.ITest<IBar>> mock;

        public NestedGenericInterface()
            => mock = new Mock<INestedGenericInterface<int>.IContainingInterface<long>.ITest<IBar>>();


        public IMock<INestedGenericInterface<int>.IContainingInterface<long>.ITest<IBar>> Context => mock;

        public INestedGenericInterface<int>.IContainingInterface<long>.ITest<IBar> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }


}
