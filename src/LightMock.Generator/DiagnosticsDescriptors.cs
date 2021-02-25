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

        public static readonly DiagnosticDescriptor KPropertyExpressionMustHaveUniqueId =
            new DiagnosticDescriptor(
                "SPG006",
                "lambda expression already generated for property setter with same unique id",
                "The method call {0} must be placed on a separate line, or you must set both unique parts of the identifier",
                "Usage", DiagnosticSeverity.Error,
                isEnabledByDefault: true
                );


        public static readonly DiagnosticDescriptor KLambdaAssignmentNotFound =
            new DiagnosticDescriptor(
                "SPG007",
                "lambda is not contains property assignment",
                "The lambda must contain property assignment",
                "Usage", DiagnosticSeverity.Error,
                isEnabledByDefault: true
                );
    }
}
