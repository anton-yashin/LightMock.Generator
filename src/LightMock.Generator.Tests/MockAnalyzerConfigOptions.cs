using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace LightMock.Generator.Tests
{
    sealed class MockAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        public static MockAnalyzerConfigOptions Empty { get; }
            = new MockAnalyzerConfigOptions(ImmutableDictionary<string, string>.Empty);

        private readonly ImmutableDictionary<string, string> backing;

        public MockAnalyzerConfigOptions(ImmutableDictionary<string, string> backing)
            => this.backing = backing;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
            => backing.TryGetValue(key, out value);
    }
}
