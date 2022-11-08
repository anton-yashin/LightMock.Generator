using Xunit;

namespace LightMock.Generator.Tests.BaseTests
{
    public class TheTests
    {
        [Fact]
        public void IsAnyValue_ReturnsDefaultValue()
        {
            Assert.Equal(default(string), The<string>.IsAnyValue);
        }

        [Fact]
        public void Reference_IsAny_ReturnsDefaultValue()
        {
            Assert.Equal(default(string), The<string>.Reference.IsAny.Value);
        }

        [Fact]
        public void Reference_Is_AnyPredicate_ReturnsDefaultValue()
        {
            Assert.Equal(default(string), The<string>.Reference.Is(s => true).Value);
        }

        [Fact]
        public void Is_AnyPredicate_ReturnsDefaultValue()
        {
            Assert.Equal(default(string), The<string>.Is(s => true));
        }

    }
}