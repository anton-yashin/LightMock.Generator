using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IAssertSet_WhenAny
    {
        public string GetAndSet { get; set; }
        public string SetOnly { set; }
    }
}
