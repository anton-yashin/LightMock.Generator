using LightMock.Generator.Locators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LightMock.Generator
{
    sealed class SyntaxHelpers
    {
        private readonly string dcgaName;
        private readonly string dcgaNamespace;
        private readonly TypeMatcher mockContextMatcher;
        private readonly TypeMatcher mockInterfaceMatcher;
        private readonly string doatName;
        private readonly string doatNamespace;

        public SyntaxHelpers()
        {
            mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            mockInterfaceMatcher = new TypeMatcher(typeof(IAdvancedMockContext<>));
            var disableCodeGenerationAttributeType = typeof(DisableCodeGenerationAttribute);
            dcgaName = disableCodeGenerationAttributeType.Name;
            dcgaNamespace = disableCodeGenerationAttributeType.Namespace;
            var dontOverrideAttributeType = typeof(DontOverrideAttribute);
            doatName = dontOverrideAttributeType.Name;
            doatNamespace = dontOverrideAttributeType.Namespace;
        }

        public static GenericNameSyntax? GetMockSymbol(SyntaxNode node)
        {
            switch (node)
            {
                case ObjectCreationExpressionSyntax { Type: GenericNameSyntax gns }:
                    return gns;
                case ObjectCreationExpressionSyntax { Type: QualifiedNameSyntax { Right: GenericNameSyntax gns } }:
                    return gns;
            }
            return null;
        }

        public static bool IsMock(SyntaxNode node)
        {
            var gns = GetMockSymbol(node);
            return gns != null && gns.Identifier.ValueText == "Mock" && gns.TypeArgumentList.Arguments.Any();
        }

        public static bool IsDisableCodeGenerationAttribute(AttributeSyntax attributeSyntax)
        {
            const string KDisableCodeGenerationAttribute = nameof(DisableCodeGenerationAttribute);
            const string KDisableCodeGeneration = "DisableCodeGeneration";
#if DEBUG
            if (KDisableCodeGenerationAttribute != KDisableCodeGeneration + nameof(Attribute))
                throw new InvalidProgramException($@"constant {nameof(KDisableCodeGeneration)} is not valid");
#endif
            switch (attributeSyntax.Name.ToString())
            {
                case KDisableCodeGeneration:
                case KDisableCodeGenerationAttribute:
                    return true;
            }
            return false;
        }

        public bool IsDisableCodeGenerationAttribute(SemanticModel semanticModel, AttributeSyntax attributeSyntax)
        {
            if (IsDisableCodeGenerationAttribute(attributeSyntax))
            {
                var si = semanticModel.GetSymbolInfo(attributeSyntax);
                if (si.Symbol is IMethodSymbol methodSymbol
                    && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == dcgaName
                    && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == dcgaNamespace)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDontOverrideAttribute(AttributeSyntax attributeSyntax)
        {
            const string KDontOverrideAttribute = nameof(DontOverrideAttribute);
            const string KDontOverride = "DontOverride";
#if DEBUG
            if (KDontOverrideAttribute != KDontOverride + nameof(Attribute))
                throw new InvalidProgramException($@"constant {nameof(KDontOverride)} is not valid");
#endif
            switch (attributeSyntax.Name.ToString())
            {
                case KDontOverride:
                case KDontOverrideAttribute:
                    return true;
            }
            return false;
        }

        public static bool IsArrangeInvocation(InvocationExpressionSyntax ies)
        {
            if (ies.Expression is MemberAccessExpressionSyntax maes)
            {
                switch (maes.Name.ToString())
                {
                    case nameof(AbstractMockNameofProvider.ArrangeSetter):
                    case nameof(AbstractMockNameofProvider.AssertSet):
                        return true;
                }
            }
            return false;
        }

        public CandidateInvocation ConvertToInvocation(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var candidateInvocation = (InvocationExpressionSyntax)node;

            var methodSymbol = semanticModel.GetSymbolInfo(candidateInvocation, cancellationToken).Symbol as IMethodSymbol;

            if (methodSymbol != null
                && (mockContextMatcher.IsMatch(methodSymbol.ContainingType)
                    || mockInterfaceMatcher.IsMatch(methodSymbol.ContainingType)))
            {
                switch (methodSymbol.Name)
                {
                    case nameof(AbstractMockNameofProvider.ArrangeSetter):
                        return new CandidateInvocation(methodSymbol, candidateInvocation, candidateInvocation);
                    case nameof(AbstractMockNameofProvider.AssertSet):
                        return new CandidateInvocation(methodSymbol, candidateInvocation, candidateInvocation);
                }
            }
            return new CandidateInvocation(null, null, candidateInvocation);
        }

        public INamedTypeSymbol? CovertToDontOverride(SemanticModel semanticModel, AttributeSyntax @as)
        {
            TypeSyntax? type;
            if (semanticModel.GetSymbolInfo(@as).Symbol is IMethodSymbol methodSymbol
                && methodSymbol.ToDisplayString(SymbolDisplayFormats.Namespace) == doatName
                && methodSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace) == doatNamespace
                && (type = TypeOfLocator.Locate(@as)?.Type) != null
                && semanticModel.GetSymbolInfo(type).Symbol is INamedTypeSymbol typeSymbol)
            {
                return typeSymbol;
            }
            return null;
        }


    }
}
