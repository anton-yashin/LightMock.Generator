using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IArrangeSetter_When
    { 
        public string GetAndSet { get; set; }
        public string SetOnly { set; }
    }
}
