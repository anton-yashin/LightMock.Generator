using System;

namespace LightMock.Generator.Tests.Mock
{
    public delegate void EventHandler<T>(object source, T data);

    public interface IInterfaceWithEventSource
    {
        event EventHandler<int> OnEvent;
    }
}
