using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Interface
{
    public delegate void EventHandlerGenericClass<T>(object source, T data);

    public interface IGenericInterfaceWithGenericEvent<T>
    {
        event EventHandlerGenericClass<T> OnEvent;
    }
}
