namespace LightMock.Generator.Tests.Interface
{
    public delegate void EventHandlerGenericClass<T>(object source, T data);

    public interface IEventSourceGenericClass<T>
    {
        event EventHandlerGenericClass<T> OnEvent;
    }
}
