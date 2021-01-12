using System;

namespace LightMock.Generator.Tests.Mock.Namespace1
{
    public class MultipleNamespacesArgument
    { }
}


namespace LightMock.Generator.Tests.Mock.Namespace2
{
    using Namespace1;

    public interface IInterfaceWithMultipleNamespaces
    {
        void DoSomething(MultipleNamespacesArgument someClass);
        MultipleNamespacesArgument GetSomething();
        MultipleNamespacesArgument SomeProperty { get; set; }
    }

}