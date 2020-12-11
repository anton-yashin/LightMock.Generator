using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaticProxy
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

    }
}
