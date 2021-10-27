﻿/******************************************************************************
    MIT License

    Copyright (c) 2021 Anton Yashin

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*******************************************************************************
    https://github.com/anton-yashin/
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightMock.Generator
{
    sealed class LightMockSyntaxReceiver : ISyntaxContextReceiver
    {
        private readonly TypeMatcher mockContextMatcher;
        private readonly TypeMatcher mockInterfaceMatcher;
        private readonly List<INamedTypeSymbol> processedTypes;
        private readonly string multicastDelegateNameSpaceAndName;

        public LightMockSyntaxReceiver()
        {
            mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            mockInterfaceMatcher = new TypeMatcher(typeof(IAdvancedMockContext<>));
            processedTypes = new List<INamedTypeSymbol>();
            var multicastDelegateType = typeof(MulticastDelegate);
            multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;
        }

        public bool DisableCodeGeneration { get; private set; }
        public List<AttributeSyntax> DontOverrideAttributes { get; } = new List<AttributeSyntax>();
        public List<InvocationExpressionSyntax> ArrangeInvocations { get; } = new();

        public List<INamedTypeSymbol> Delegates { get; } = new();
        public List<INamedTypeSymbol> Interfaces { get; } = new();
        public List<(GenericNameSyntax mock, INamedTypeSymbol mockedType)> AbstractClasses { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            switch (context.Node)
            {
                case ObjectCreationExpressionSyntax { Type: GenericNameSyntax gns }
                when IsMock(gns):
                    AddCandidateMock(gns, context.SemanticModel);
                    break;
                case ObjectCreationExpressionSyntax { Type: QualifiedNameSyntax { Right: GenericNameSyntax gns } }
                when IsMock(gns):
                    AddCandidateMock(gns, context.SemanticModel);
                    break;
                case AttributeSyntax @as when IsDontOverrideAttribute(@as):
                    DontOverrideAttributes.Add(@as);
                    break;
                case AttributeSyntax @as:
                    DisableCodeGeneration = DisableCodeGeneration || IsDisableCodeGenerationAttribute(context.SemanticModel, @as);
                    break;
                case InvocationExpressionSyntax ies when IsArrangeInvocation(ies):
                    ArrangeInvocations.Add(ies);
                    break;
            }
        }

        void AddCandidateMock(GenericNameSyntax candidateGeneric, SemanticModel semanticModel)
        {
            var mockContainer = semanticModel.GetSymbolInfo(candidateGeneric).Symbol
                as INamedTypeSymbol;
            var mcbt = mockContainer?.BaseType;
            if (mcbt != null
                && mockContextMatcher.IsMatch(mcbt)
                && mcbt.TypeArguments.FirstOrDefault() is INamedTypeSymbol mockedType
                && processedTypes.Contains(mockedType.OriginalDefinition) == false)
            {
                processedTypes.Add(mockedType.OriginalDefinition);

                var mtbt = mockedType.BaseType;
                if (mtbt != null)
                {
                    if (mtbt.ToDisplayString(SymbolDisplayFormats.Namespace) == multicastDelegateNameSpaceAndName)
                        Delegates.Add(mockedType);
                    else
                        AbstractClasses.Add((candidateGeneric, mockedType));
                }
                else
                    Interfaces.Add(mockedType);
            }
        }

        internal static bool IsMock(GenericNameSyntax gns)
            => gns.Identifier.ValueText == "Mock" && gns.TypeArgumentList.Arguments.Any();

        internal static bool IsDisableCodeGenerationAttribute(AttributeSyntax attributeSyntax)
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

        internal static bool IsDisableCodeGenerationAttribute(SemanticModel semanticModel, AttributeSyntax attributeSyntax)
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

        internal static bool IsDontOverrideAttribute(AttributeSyntax attributeSyntax)
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

        internal static bool IsArrangeInvocation(InvocationExpressionSyntax ies)
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