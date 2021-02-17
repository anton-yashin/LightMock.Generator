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

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is CSharpSyntaxNode cssn)
                cssn.Accept(this);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            {
                if (node.Type is GenericNameSyntax gns && gns.Identifier.ValueText == "Mock" && gns.TypeArgumentList.Arguments.Any())
                {
                    CandidateMocks.Add(gns);
                }
            }
            {
                if (node.Type is QualifiedNameSyntax qns && qns.Right is GenericNameSyntax gns
                    && gns.Identifier.ValueText == "Mock" && gns.TypeArgumentList.Arguments.Any())
                {
                    CandidateMocks.Add(gns);
                }
            }
        }

        const string KDisableCodeGenerationAttribute = nameof(DisableCodeGenerationAttribute);
        const string KDisableCodeGeneration = "DisableCodeGeneration";
        const string KDontOverrideAttribute = nameof(DontOverrideAttribute);
        const string KDontOverride = "DontOverride";


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
    }
}