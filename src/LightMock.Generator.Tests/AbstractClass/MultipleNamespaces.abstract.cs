namespace LightMock.Generator.Tests.AbstractClass.Namespace1
{
    public class AMultipleNamespacesArgument
    {
    }
}

namespace LightMock.Generator.Tests.AbstractClass.Namespace2
{
    using Namespace1;

    public abstract class AMultipleNamespaces
    {
        public abstract void DoSomething(AMultipleNamespacesArgument someClass);
        public abstract AMultipleNamespacesArgument GetSomething();
        public abstract AMultipleNamespacesArgument SomeProperty { get; set; }

        protected abstract void ProtectedDoSomething(AMultipleNamespacesArgument someClass);
        protected abstract AMultipleNamespacesArgument ProtectedGetSomething();
        protected abstract AMultipleNamespacesArgument ProtectedSomeProperty { get; set; }
    }
}
