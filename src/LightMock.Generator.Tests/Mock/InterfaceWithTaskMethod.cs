using System;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Mock
{
    public interface IInterfaceWithTaskMethod
    {
        Task FooAsync();
        Task<int> BarAsync();
        Task<T> BazAsync<T>();
    }
}
