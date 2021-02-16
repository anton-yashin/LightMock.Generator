using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LightMock.Generator
{
    sealed class ExclusionTypeFinder : CSharpSyntaxWalker
    {
        TypeSyntax? type;

        private ExclusionTypeFinder() { }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            type = node.Type;
            base.VisitTypeOfExpression(node);
        }

        public static TypeSyntax? FindAt(SyntaxNode node)
        {
            var @this = new ExclusionTypeFinder();
            @this.Visit(node);
            return @this.type;
        }
    }
}
