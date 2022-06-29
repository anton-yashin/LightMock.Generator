using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;

namespace LightMock.Generator.Tests.TestAbstractions
{
    public record class CompilationResult(ImmutableArray<Diagnostic> diagnostics, bool success, byte[] assembly);
}
