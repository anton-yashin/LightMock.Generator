using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Testee
{
    [GenerateMock]
    public partial class EventSourceGenericClass<T> : IEventSourceGenericClass<T>
    {
    }
}
