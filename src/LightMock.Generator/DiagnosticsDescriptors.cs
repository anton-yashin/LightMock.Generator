using Microsoft.CodeAnalysis;

namespace LightMock.Generator
{
    static class DiagnosticsDescriptors
    {
        public static readonly DiagnosticDescriptor KNoAttributeErrorDescriptor =
            new DiagnosticDescriptor(
                "SPG001",
                "internal error",
                "attribute {0} is missing",
                "Usage", DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor KNoPartialKeyworkErrorDescriptor =
            new DiagnosticDescriptor(
                "SPG002",
                "no partial keword",
                "The type {0} should be declared with the 'partial' keyword " +
                "as it is annotated with the [MockGeneratedAttribute] attribute",
                "Usage", DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor KNoInterfaceErrorDescriptor =
            new DiagnosticDescriptor(
                "SPG003",
                "no interface",
                "The type {0} should be directly implement at least one interface " +
                "as it is annotated with the [MockGeneratedAttribute] attribute",
                "Usage", DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor KTooManyInterfacesWarningDescriptor =
            new DiagnosticDescriptor(
                "SPG004",
                "too many interfaces",
                "The type {0} implements more than one interface, only first one will be processed",
                "Usage", DiagnosticSeverity.Warning,
                isEnabledByDefault: true
                );
    }
}
