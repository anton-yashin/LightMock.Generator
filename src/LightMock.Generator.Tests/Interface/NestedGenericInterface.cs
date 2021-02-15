using System;

namespace LightMock.Generator.Tests.Interface
{
    public interface INestedGenericInterface<out T1>
        where T1 : struct, IEquatable<T1>
    {
        public interface IContainingInterface<T2>
        {
            public interface ITest<in T3>
                where T3 : class, IFoo
            {
                T1 Foo(T2? p);
                T1 Bar<T4>(T2? p)
                    where T4 : IEquatable<T4>;

                T2 Baz(T3? p);
            }
        }
    }
}
