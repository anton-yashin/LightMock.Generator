using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IArrangeSetter_OnAny
    { 
        string GetAndSet { get; set; }
        string SetOnly { set; }
    }
}
