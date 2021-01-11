using System;

namespace LightMock.Generator.Tests.Mock
{
    public interface IInterfaceWithBasicProperty
    {
        int OnlyGet { get; }
        int GetAndSet { get; set; }
    }
}
