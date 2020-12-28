using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AEventSource
    {
        public abstract event EventHandler<int>? OnPublicEvent;
        protected abstract event EventHandler<object>? OnProtectedEvent;
        public virtual event Action? OnPublicVirtualAction;
        protected virtual event Action? OnProtectedVirtualAction;
    }
}
