using System;

namespace StaticProxy.Tests.Testee
{
    public interface ISomeInterface
    {
        void DoSomething(int someParam);
        int ReturnSomething();
    }

    [MockGenerated]
    public partial class SomeClass : ISomeInterface
    {
    }
}
