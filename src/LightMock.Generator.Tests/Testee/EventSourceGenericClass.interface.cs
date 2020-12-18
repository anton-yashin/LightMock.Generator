namespace LightMock.Generator.Tests.Testee
{
    public delegate void EventHandlerGenericClass<T>(object source, T data);

    public interface IEventSourceGenericClass<T>
    {
        event EventHandlerGenericClass<T> OnEvent;
    }
}
