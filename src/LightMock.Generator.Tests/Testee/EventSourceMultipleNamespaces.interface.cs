namespace EventNamespace1
{
    public delegate void EventHandler<T>(object source, T data);
}

namespace EventNamespace2
{
    using EventNamespace1;

    public interface IEventSourceMultipleNamespaces
    {
        event EventHandler<int> OnEvent;
    }
}
