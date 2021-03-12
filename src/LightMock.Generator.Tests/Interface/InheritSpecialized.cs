using System;
using System.Collections.Generic;

namespace LightMock.Generator.Tests.Interface
{
    public sealed class SpecializationTag { }

    public interface IInheritSpecialized2<T> : IList<SpecializationTag>, ICollection<SpecializationTag>, IEnumerable<SpecializationTag>
    {
        T Function();
        void Action(T a);
        T Property { get; set; }
    }

    public interface IInheritSpecialized : IInheritSpecialized2<SpecializationTag>, IList<SpecializationTag>, ICollection<SpecializationTag>, IEnumerable<SpecializationTag>
    {
        string Foo();
        void Bar(string a);
    }
}
