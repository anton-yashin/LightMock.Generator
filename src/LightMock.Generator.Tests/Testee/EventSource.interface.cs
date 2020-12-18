namespace LightMock.Generator.Tests.Testee
{
    public delegate void EventHandler<T>(object source, T data);

    public interface IEventSource
    {
        event EventHandler<int> OnEvent;
    }
}
