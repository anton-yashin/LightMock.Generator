using System;

namespace StaticProxy.Tests.Testee
{
    public interface ISomeInterface
    {
        void DoSomething();        
    }

    [MockGenerated]
    public partial class SomeClass : ISomeInterface
    {
    }
}
