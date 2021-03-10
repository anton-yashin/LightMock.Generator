using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IArrangeSetter_On
    { 
        public string GetAndSet { get; set; }
        public string SetOnly { set; }
    }
}
