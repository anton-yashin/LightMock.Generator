using System;

namespace LightMock.Generator.Tests.Mock.EventNamespace1
{
    public delegate void EventHandler<T>(object source, T data);
}


namespace LightMock.Generator.Tests.Mock.EventNamespace2
{
    using EventNamespace1;

    public interface IInterfaceWithEventSourceAndMultipleNamespaces
    {
        event EventHandler<int> OnEvent;
    }
}

