namespace LightMock.Generator.Tests.AbstractClass
{
    public delegate void EventHandlerGenericClass<T>(object source, T data);

    public abstract class AEventSourceGenericClass<T>
    {
#pragma warning disable CS0067 // The event is never used
        public event EventHandlerGenericClass<T>? OnPublicEvent;
        public abstract event EventHandlerGenericClass<T>? OnAbstractEvent;
        public virtual event EventHandlerGenericClass<T>? OnVirtualEvent;

        protected event EventHandlerGenericClass<T>? OnProtectedEvent;
        protected abstract event EventHandlerGenericClass<T>? OnProtectedAbstractEvent;
        protected virtual event EventHandlerGenericClass<T>? OnProtectedVirtualEvent;
#pragma warning restore CS0067 // The event is never used
    }
}
