using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface INestedInterface
    {
        public interface ITest
        {
            void Foo();
        }
    }
}
