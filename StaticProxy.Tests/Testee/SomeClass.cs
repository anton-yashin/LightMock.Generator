using System;

namespace StaticProxy.Tests.Testee
{
    public interface ISomeInterface
    {
        void DoSomething(int someParam);
        int ReturnSomething();
    }

    [GenerateMock]
    public partial class SomeClass : ISomeInterface
    {
    }
}
