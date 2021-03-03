using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface IArrangeSetter
    { 
        string GetAndSet { get; set; }
        string Set { set; }
    }
}
