using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassWithEnumerableResult : ITestScript<AAbstractClassWithEnumerableResult>
    {
        private readonly Mock<AAbstractClassWithEnumerableResult> mock;

        public AbstractClassWithEnumerableResult()
            => mock = new Mock<AAbstractClassWithEnumerableResult>();

        public IMock<AAbstractClassWithEnumerableResult> Context => mock;

        public AAbstractClassWithEnumerableResult MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
