using System;
using Xunit;

namespace LightMock.Generator.Tests
{
    public class TypeCacher_Tests
    {
        [Fact]
        public void SameTags_ActivatesObjects_OfSameType()
        {
            Type TypeResolver() => typeof(SameTags_ActivatesObjects_OfSameType_TestData);

            var tc1 = new TypeCacher<SameTags_ActivatesObjects_OfSameType_TestData>();
            var tc2 = new TypeCacher<SameTags_ActivatesObjects_OfSameType_TestData>();
            var o1 = tc1.Activate(TypeResolver);
            var o2 = tc2.Activate(TypeResolver);

            Assert.Equal(o1.GetType(), o2.GetType());
        }

        sealed class SameTags_ActivatesObjects_OfSameType_TestData { }

        [Fact]
        public void SameTags_InvokesResover_Once()
        {
            int invoked = 0;
            Type TypeResolver() { invoked++; return typeof(SameTags_InvokesResover_Once_TestData); }

            var tc1 = new TypeCacher<SameTags_InvokesResover_Once_TestData>();
            var tc2 = new TypeCacher<SameTags_InvokesResover_Once_TestData>();
            var o1 = tc1.Activate(TypeResolver);
            var o2 = tc2.Activate(TypeResolver);

            Assert.Equal(1, invoked);
        }

        sealed class SameTags_InvokesResover_Once_TestData { }

        [Fact]
        public void DifferentTags_ActivatesDifferentObjects()
        {
            Type TypeResolver1() => typeof(DifferentTags_ActivatesDifferentObjects_TestData1);
            Type TypeResolver2() => typeof(DifferentTags_ActivatesDifferentObjects_TestData2);

            var tc1 = new TypeCacher<DifferentTags_ActivatesDifferentObjects_TestData1>();
            var tc2 = new TypeCacher<DifferentTags_ActivatesDifferentObjects_TestData2>();
            var o1 = tc1.Activate(TypeResolver1);
            var o2 = tc2.Activate(TypeResolver2);

            Assert.NotEqual(o1.GetType(), o2.GetType());
        }

        sealed class DifferentTags_ActivatesDifferentObjects_TestData1 { }
        sealed class DifferentTags_ActivatesDifferentObjects_TestData2 { }
    }
}
