using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IAssertSet
    {
        public string GetAndSet { get; set; }
        public string SetOnly { set; }
    }
}
