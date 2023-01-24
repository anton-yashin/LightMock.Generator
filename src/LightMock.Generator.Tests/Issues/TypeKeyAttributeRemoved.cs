using System;

namespace LightMock.Generator.Tests.Issues
{
    public abstract class ATypeKeyAttributeRemoved
    {
        public abstract void Foo();
    }

    public abstract class ATypeKeyAttributeRemoved<T>
    {
        public abstract void Foo(T bar);
    }

    public interface ITypeKeyAttributeRemoved
    {
        void Foo();
    }

    public interface ITypeKeyAttributeRemoved<T>
    {
        void Foo(T bar);
    }
}
