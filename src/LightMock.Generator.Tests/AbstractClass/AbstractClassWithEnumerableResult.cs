using LightMock.Generator.Tests.TestAbstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAbstractClassWithEnumerableResult
    {
        public abstract IEnumerable<int> CollectionProperty { get; }
        public abstract IEnumerable<int> GetCollection();
        public abstract IEnumerable<T> GetGenericCollection<T>();
        public abstract Task<IEnumerable<int>> GetCollectionAsync();
        public abstract Task<IEnumerable<T>> GetGenericCollectionAsync<T>();
    }
}
