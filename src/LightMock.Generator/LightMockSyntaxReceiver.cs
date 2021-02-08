﻿using System.Collections.Generic;
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

        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
        public List<GenericNameSyntax> CandidateMocks { get; } = new List<GenericNameSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is CSharpSyntaxNode cssn)
                cssn.Accept(this);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.AttributeLists.Count > 0)
                CandidateClasses.Add(node);
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
    }
}