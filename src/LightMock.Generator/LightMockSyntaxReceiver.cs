/******************************************************************************
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
using LightMock.Generator.Locators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightMock.Generator
{
    sealed class LightMockSyntaxReceiver : ISyntaxContextReceiver
    {
        private readonly TypeMatcher mockContextMatcher;
        private readonly List<INamedTypeSymbol> processedTypes;
        private readonly string multicastDelegateNameSpaceAndName;
        private readonly SyntaxHelpers syntaxHelpers;

        public LightMockSyntaxReceiver()
        {
            mockContextMatcher = new TypeMatcher(typeof(AbstractMock<>));
            processedTypes = new List<INamedTypeSymbol>();
            var multicastDelegateType = typeof(MulticastDelegate);
            multicastDelegateNameSpaceAndName = multicastDelegateType.Namespace + "." + multicastDelegateType.Name;
            syntaxHelpers = new SyntaxHelpers();
        }

        public bool DisableCodeGeneration { get; private set; }
        public List<INamedTypeSymbol> DontOverrideTypes { get; } = new();
        public List<CandidateInvocation> CandidateInvocations { get; } = new();

        public List<INamedTypeSymbol> Delegates { get; } = new();
        public List<INamedTypeSymbol> Interfaces { get; } = new();
        public List<(GenericNameSyntax mock, INamedTypeSymbol mockedType)> AbstractClasses { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            GenericNameSyntax? gns;
            if ((gns = SyntaxHelpers.GetMockSymbol(context.Node)) != null)
            {
                AddCandidateMock(gns, context.SemanticModel);
            }
            else if (context.Node is AttributeSyntax @as)
            {
                if (SyntaxHelpers.IsDontOverrideAttribute(@as))
                    AddDontOverrideType(context.SemanticModel, @as);
                else if (syntaxHelpers.IsDisableCodeGenerationAttribute(context.SemanticModel, @as))
                    DisableCodeGeneration = true;
            }
            else if (context.Node is InvocationExpressionSyntax ies && SyntaxHelpers.IsArrangeInvocation(ies))
            {
                CandidateInvocations.Add(
                    syntaxHelpers.ConvertToInvocation(
                        context.Node, context.SemanticModel, default));
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

        private void AddDontOverrideType(SemanticModel semanticModel, AttributeSyntax @as)
        {
            var typeSymbol = syntaxHelpers.CovertToDontOverride(semanticModel, @as);
            if (typeSymbol != null)
                DontOverrideTypes.Add(typeSymbol);
        }
    }
}