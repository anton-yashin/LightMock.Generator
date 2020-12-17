namespace MultipleNamespaces1
{
    public class MultipleNamespacesArgument
    { }
}

namespace MultipleNamespaces2
{
    using MultipleNamespaces1;

    public interface IMultipleNamespaces
    {
        void DoSomething(MultipleNamespacesArgument someClass);
        MultipleNamespacesArgument GetSomething();
        MultipleNamespacesArgument SomeProperty { get; set; }
    }
}
