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
        private CancellationToken cancellationToken;

        public LightMockSyntaxReceiver(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public List<GenericNameSyntax> CandidateMocks { get; } = new List<GenericNameSyntax>();
        public List<AttributeSyntax> DisableCodeGenerationAttributes { get; } = new List<AttributeSyntax>();
        public List<AttributeSyntax> DontOverrideAttributes { get; } = new List<AttributeSyntax>();
        public List<InvocationExpressionSyntax> ArrangeInvocations { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
            if (node.Expression is MemberAccessExpressionSyntax maes
                && maes.Name.ToString() == nameof(AbstractMockNameofProvider.ArrangeSetter))
            {
                ArrangeInvocations.Add(node);
            }
        }
    }
}