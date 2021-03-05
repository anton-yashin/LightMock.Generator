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
        public void Is_AnyPredicate_ReturnsDefaultValue()
        {
            Assert.Equal(default(string), The<string>.Is(s => true));
        }
    }
}