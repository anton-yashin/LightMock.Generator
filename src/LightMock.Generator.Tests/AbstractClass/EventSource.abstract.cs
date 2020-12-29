using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AEventSource
    {
        public abstract event EventHandler<int>? OnPublicEvent;
        protected abstract event EventHandler<object>? OnProtectedEvent;
#pragma warning disable CS0067 // Field not used
        public virtual event Action? OnPublicVirtualAction;
        protected virtual event Action? OnProtectedVirtualAction;

        public event Action? NonVirtualNonAbstractPublicEvent;
        protected event Action? NonVirtualNonAbstractProtectedEvent;
#pragma warning restore CS0067 // Field not used
    }
}
