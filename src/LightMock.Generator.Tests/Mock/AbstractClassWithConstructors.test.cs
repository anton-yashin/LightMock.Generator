﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Mock
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