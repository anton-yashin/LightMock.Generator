using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ANoPartialKeyworkError 
    {
        abstract void DoSomething();
    }

    [GenerateMock]
    class NoPartialKeyworkError : ANoPartialKeyworkError
    {
    }
}
