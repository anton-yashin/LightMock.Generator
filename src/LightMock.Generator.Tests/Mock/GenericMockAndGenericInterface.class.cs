﻿using System;

namespace LightMock.Generator.Tests.Mock
{
    public class GenericMockAndGenericInterface<T> : ITestScript<IGenericMockAndGenericInterface<T>>
    {
        private readonly Mock<IGenericMockAndGenericInterface<T>> mock;

        public GenericMockAndGenericInterface()
            => this.mock = new Mock<IGenericMockAndGenericInterface<T>>();

        public MockContext<IGenericMockAndGenericInterface<T>> Context => mock;

        public IGenericMockAndGenericInterface<T> MockObject => mock.Object;

        public int DoRun()
        {
            return 42;
        }
    }
}
