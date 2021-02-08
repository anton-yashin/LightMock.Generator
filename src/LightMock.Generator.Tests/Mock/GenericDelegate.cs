using System;

namespace LightMock.Generator.Tests.Mock
{
    public delegate TResult SomeGenericDelegate<in T1, in T2, in T3, out TResult>(T1 a1, T2 a2, T3 a3)
        where T1 : class
        where T2 : struct, IEquatable<T2>
        where T3 : IEquatable<T3>;
}
