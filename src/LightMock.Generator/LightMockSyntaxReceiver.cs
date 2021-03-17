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
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightMock.Generator
{
    sealed class LightMockSyntaxReceiver : CSharpSyntaxVisitor, ISyntaxReceiver
    {
        public LightMockSyntaxReceiver() { }

        public List<GenericNameSyntax> CandidateMocks { get; } = new List<GenericNameSyntax>();
        public List<AttributeSyntax> DisableCodeGenerationAttributes { get; } = new List<AttributeSyntax>();
        public List<AttributeSyntax> DontOverrideAttributes { get; } = new List<AttributeSyntax>();
        public List<InvocationExpressionSyntax> ArrangeInvocations { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is CSharpSyntaxNode cssn)
                cssn.Accept(this);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            switch (node.Type)
            {
                case GenericNameSyntax gns when IsMock(gns):
                    CandidateMocks.Add(gns);
                    break;
                case QualifiedNameSyntax qns when qns.Right is GenericNameSyntax gns && IsMock(gns):
                    CandidateMocks.Add(gns);
                    break;
            }
        }

        static bool IsMock(GenericNameSyntax gns)
            => gns.Identifier.ValueText == "Mock" && gns.TypeArgumentList.Arguments.Any();

        const string KDisableCodeGenerationAttribute = nameof(DisableCodeGenerationAttribute);
        const string KDisableCodeGeneration = "DisableCodeGeneration";
        const string KDontOverrideAttribute = nameof(DontOverrideAttribute);
        const string KDontOverride = "DontOverride";
        const string KMock = "Mock";


        public override void VisitAttribute(AttributeSyntax node)
        {
#if DEBUG
            if (KDisableCodeGenerationAttribute != KDisableCodeGeneration + nameof(Attribute))
                throw new InvalidProgramException($@"constant {nameof(KDisableCodeGeneration)} is not valid");
            if (KDontOverrideAttribute != KDontOverride + nameof(Attribute))
                throw new InvalidProgramException($@"constant {nameof(KDontOverride)} is not valid");
#endif
            switch (node.Name.ToString())
            {
                case KDisableCodeGeneration:
                case KDisableCodeGenerationAttribute:
                    DisableCodeGenerationAttributes.Add(node);
                    break;
                case KDontOverride:
                case KDontOverrideAttribute:
                    DontOverrideAttributes.Add(node);
                    break;
            }
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax maes)
            {
                switch (maes.Name.ToString())
                {
                    case nameof(AbstractMockNameofProvider.ArrangeSetter):
                    case nameof(AbstractMockNameofProvider.AssertSet):
                        ArrangeInvocations.Add(node);
                        break;
                }

            }
        }
    }
}