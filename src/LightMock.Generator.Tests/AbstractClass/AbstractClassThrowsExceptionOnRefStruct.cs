using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAbstractClassThrowsExceptionOnRefStruct
    {
        public abstract void Foo<T>(Span<T> span);
        public abstract void Bar(Span<int> span);
    }
}
