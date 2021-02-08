using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IInterfaceWithBasicProperty
    {
        int OnlyGet { get; }
        int GetAndSet { get; set; }
    }
}
