using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    sealed class SyntaxHelpers
    {
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

        public static bool IsDisableCodeGenerationAttribute(SemanticModel semanticModel, AttributeSyntax attributeSyntax)
        {
            if (IsDisableCodeGenerationAttribute(attributeSyntax))
            {
                var disableCodeGenerationAttributeType = typeof(DisableCodeGenerationAttribute);
                var dcgaName = disableCodeGenerationAttributeType.Name;
                var dcgaNamespace = disableCodeGenerationAttributeType.Namespace;
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

    }
}
