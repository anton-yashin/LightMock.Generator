
namespace LightMock.Generator.Tests.AbstractClass.Namespace3
{
    public delegate void EventHandler<T>(object source, T data);
}

namespace LightMock.Generator.Tests.AbstractClass.Namespace4
{
    using Namespace3;

    public abstract class AEventSourceMultipleNamespaces
    {
        public abstract event EventHandler<int>? OnPublicEvent;
        protected abstract event EventHandler<object>? OnProtectedEvent;
#pragma warning disable CS0067 // Field not used
        public virtual event EventHandler<int>? OnPublicVirtualEvent;
        protected virtual event EventHandler<object>? OnProtectedVirtualEvent;

        public event EventHandler<int>? NonVirtualNonAbstractPublicEvent;
        protected event EventHandler<object>? NonVirtualNonAbstractProtectedEvent;
#pragma warning restore CS0067 // Field not used
    }
}
