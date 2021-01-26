using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace LightMock.Generator.Tests
{
    sealed class MockAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly ImmutableDictionary<object, AnalyzerConfigOptions> otherOptions;

        public MockAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions)
            : this(globalOptions, ImmutableDictionary<object, AnalyzerConfigOptions>.Empty)
        { }

        public MockAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions,
            ImmutableDictionary<object, AnalyzerConfigOptions> otherOptions)
        {
            GlobalOptions = globalOptions;
            this.otherOptions = otherOptions;
        }

        public static MockAnalyzerConfigOptionsProvider Empty { get; }
            = new MockAnalyzerConfigOptionsProvider(
                MockAnalyzerConfigOptions.Empty,
                ImmutableDictionary<object, AnalyzerConfigOptions>.Empty);

        public override AnalyzerConfigOptions GlobalOptions { get; }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => GetOptionsPrivate(tree);

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => GetOptionsPrivate(textFile);

        AnalyzerConfigOptions GetOptionsPrivate(object o)
            => otherOptions.TryGetValue(o, out var options) ? options : MockAnalyzerConfigOptions.Empty;
    }
}
