using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AIndexer<T>
    {
        public abstract T this[int index] { get; set; }
    }
}
