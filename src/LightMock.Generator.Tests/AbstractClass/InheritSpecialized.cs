using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public sealed class SpecializationTag { }

    public abstract class AInheritSpecialized<T>
    {
        public abstract T Function();
        public abstract void Action(T a);
        public abstract T Property { get; set; }
    }

    public abstract class AInheritSpecialized : AInheritSpecialized<SpecializationTag>
    {
        public abstract string Foo();
        public abstract void Bar(string a);
    }
}
