using System;

namespace LightMock.Generator.Tests.Interface
{
    public delegate void EventHandler<T>(object source, T data);

    public interface IInterfaceWithEventSource
    {
        event EventHandler<int> OnEvent;
    }
}
