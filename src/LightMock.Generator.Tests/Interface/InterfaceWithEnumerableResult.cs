using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Interface
{
    public interface IInterfaceWithEnumerableResult
    {
        IEnumerable<int> CollectionProperty { get; }
        IEnumerable<int> GetCollection();
        IEnumerable<T> GetGenericCollection<T>();
        Task<IEnumerable<int>> GetCollectionAsync();
        Task<IEnumerable<T>> GetGenericCollectionAsync<T>();
    }
}
