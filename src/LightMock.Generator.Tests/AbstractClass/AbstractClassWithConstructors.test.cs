using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightMock.Generator.Tests.TestAbstractions;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AbstractClassWithConstructors : ITestScript<AAbstractClassWithConstructors>
    {
        private readonly Mock<AAbstractClassWithConstructors> mock;

        public AbstractClassWithConstructors()
            => mock = new Mock<AAbstractClassWithConstructors>((int)1234, (IEnumerable<object>)null);

        public IMock<AAbstractClassWithConstructors> Context => mock;

        public AAbstractClassWithConstructors MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
