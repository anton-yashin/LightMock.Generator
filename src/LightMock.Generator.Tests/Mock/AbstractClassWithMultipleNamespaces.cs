using System;

namespace LightMock.Generator.Tests.Mock.Namespace2
{
    using Namespace1;

    public abstract class AAbstractClassWithMultipleNamespaces
    {
        public abstract void DoSomething(MultipleNamespacesArgument someClass);
        public abstract MultipleNamespacesArgument GetSomething();
        public abstract MultipleNamespacesArgument SomeProperty { get; set; }

        protected abstract void ProtectedDoSomething(MultipleNamespacesArgument someClass);
        protected abstract MultipleNamespacesArgument ProtectedGetSomething();
        protected abstract MultipleNamespacesArgument ProtectedSomeProperty { get; set; }

        public void InvokeProtectedDoSomething(MultipleNamespacesArgument someClass) => ProtectedDoSomething(someClass);
        public MultipleNamespacesArgument InvokeProtectedGetSomething() => ProtectedGetSomething();
        public MultipleNamespacesArgument InvokeProtectedSomeProperty 
        {
            get => ProtectedSomeProperty;
            set => ProtectedSomeProperty = value;
        }
    }
}
