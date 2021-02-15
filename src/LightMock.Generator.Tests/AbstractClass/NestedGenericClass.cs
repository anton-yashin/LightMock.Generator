using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ANestedGenericClass<T1>
        where T1 : struct, IEquatable<T1>
    {
        public abstract class AContainingClass<T2>
        {
            public abstract class ATest<T3>
                where T3 : class
            {
                public abstract T1 Foo(T2? p);
                public abstract T1 Bar<T4>(T2? p)
                    where T4 : IEquatable<T4>;

                public abstract T2 Baz(T3? p);
            }
        }
    }
}
