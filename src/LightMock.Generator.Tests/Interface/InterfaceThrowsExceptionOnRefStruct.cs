using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{

    public interface IInterfaceThrowsExceptionOnRefStruct
    {
        void Foo<T>(Span<T> span);
        void Bar(Span<int> span);
    }
}
