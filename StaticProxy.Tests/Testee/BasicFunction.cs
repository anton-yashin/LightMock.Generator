using System;

namespace StaticProxy.Tests.Testee
{
    public interface IBasicFunction
    {
        void DoSomething(int someParam);
        int ReturnSomething();
    }

    [GenerateMock]
    public partial class BasicFunction : IBasicFunction
    {
    }
}
