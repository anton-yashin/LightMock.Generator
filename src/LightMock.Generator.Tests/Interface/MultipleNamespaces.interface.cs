namespace LightMock.Generator.Tests.Interface.MultipleNamespaces1
{
    public class MultipleNamespacesArgument
    { }
}

namespace LightMock.Generator.Tests.Interface.MultipleNamespaces2
{
    using MultipleNamespaces1;

    public interface IMultipleNamespaces
    {
        void DoSomething(MultipleNamespacesArgument someClass);
        MultipleNamespacesArgument GetSomething();
        MultipleNamespacesArgument SomeProperty { get; set; }
    }
}
