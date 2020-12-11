using System;

namespace StaticProxy.Tests.Testee
{
    public interface IBasicMethod
    {
        void DoSomething(int someParam);
        int ReturnSomething();
    }

    [GenerateMock]
    public partial class BasicMethod : IBasicMethod
    {
    }
}
