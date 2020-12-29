using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Interface
{
    public interface INoPartialKeyworkError 
    {
        void DoSomething();
    }

    [GenerateMock]
    class NoPartialKeyworkError : INoPartialKeyworkError
    {
    }
}
