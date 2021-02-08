using Microsoft.CodeAnalysis;

namespace LightMock.Generator
{
    static class DiagnosticsDescriptors
    {

        public static readonly DiagnosticDescriptor KCantProcessSealedClass =
            new DiagnosticDescriptor(
                "SPG005",
                "can't process sealed class",
                "The type {0} must be not sealed",
                "Usage", DiagnosticSeverity.Error,
                isEnabledByDefault: true
                );
    }
}
