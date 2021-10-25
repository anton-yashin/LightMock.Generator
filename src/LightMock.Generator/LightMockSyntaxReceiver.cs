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
    sealed class LightMockSyntaxReceiver : CSharpSyntaxVisitor, ISyntaxContextReceiver
    {
        public LightMockSyntaxReceiver() { }

        public List<GenericNameSyntax> CandidateMocks { get; } = new List<GenericNameSyntax>();
        public List<AttributeSyntax> DisableCodeGenerationAttributes { get; } = new List<AttributeSyntax>();
        public List<AttributeSyntax> DontOverrideAttributes { get; } = new List<AttributeSyntax>();
        public List<InvocationExpressionSyntax> ArrangeInvocations { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is CSharpSyntaxNode cssn)
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

        public override void VisitAttribute(AttributeSyntax node)
        {
            if (IsDisableCodeGenerationAttribute(node))
            {
                DisableCodeGenerationAttributes.Add(node);
                return;
            }
            if (IsDontOverrideAttribute(node))
            {
                DontOverrideAttributes.Add(node);
                return;
            }
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

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (IsArrangeInvocation(node))
                ArrangeInvocations.Add(node);
        }
    }
}