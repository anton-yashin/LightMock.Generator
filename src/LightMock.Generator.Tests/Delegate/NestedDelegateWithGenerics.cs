using System;

namespace LightMock.Generator.Tests.Delegate
{
    public interface XNestedInterface<in T1>
        where T1: struct, IEquatable<T1>
    {
        public interface XContainer<T2>
        {
            public delegate void SomeDelegate<T3>(T1 a1, T2 a2, T3 a3)
                where T3 : class;
        }
    }
}
