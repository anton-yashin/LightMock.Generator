using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IAssertNoOtherCalls_Throws
    {
        string GetOnly { get; }
        string SetOnly { set; }
        string GetAndSet { get; set; }
        string Function(string a);
        void Method(string a);
        string this[string index] { get; set; }
        event EventHandler EventHandler;
    }
}
