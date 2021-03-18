using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IAssertNoOtherCalls
    {
        string GetOnly { get; }
        string SetOnly { set; }
        string GetAndSet { get; set; }

        string this[string index] { get; set; }

        string Function(string a);
        void Method(string a);
    }
}
