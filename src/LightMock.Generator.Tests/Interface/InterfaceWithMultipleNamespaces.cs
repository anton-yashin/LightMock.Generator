using System;

namespace LightMock.Generator.Tests.Interface.Namespace1
{
    public class MultipleNamespacesArgument
    { }
}


namespace LightMock.Generator.Tests.Interface.Namespace2
{
    using Namespace1;

    public interface IInterfaceWithMultipleNamespaces
    {
        void DoSomething(MultipleNamespacesArgument someClass);
        MultipleNamespacesArgument GetSomething();
        MultipleNamespacesArgument SomeProperty { get; set; }
    }

}