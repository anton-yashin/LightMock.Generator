using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class SomeBaseClass
    {
        public virtual void Quux() { }
        public abstract void Quuux();
    }

    public abstract class SkipThisClass : SomeBaseClass
    {
        public virtual void Foo() => throw new InvalidProgramException(nameof(SkipThisClass) + " called");
        public virtual int Bar() => throw new InvalidProgramException(nameof(SkipThisClass) + " called");
        public abstract void Baz();
    }

    public abstract class ADontOverrideSupport : SkipThisClass
    {
        public virtual void Quuuux() { }
        public abstract void Quuuuux();
    }
}

