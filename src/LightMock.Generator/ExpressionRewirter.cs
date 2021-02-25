﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class ExpressionRewirter
    {
        private readonly IMethodSymbol method;
        private readonly InvocationExpressionSyntax invocationExpressionSyntax;
        private readonly CSharpCompilation compilation;
        private readonly ICollection<string> uids;
        private readonly ICollection<Diagnostic> errors;
        private readonly ICollection<Diagnostic> warnings;

        public ExpressionRewirter(
            IMethodSymbol method, 
            InvocationExpressionSyntax invocationExpressionSyntax,
            CSharpCompilation compilation,
            ICollection<string> uids)
        {
            this.method = method;
            this.invocationExpressionSyntax = invocationExpressionSyntax;
            this.compilation = compilation;
            this.uids = uids;

            this.errors = new List<Diagnostic>();
            this.warnings = new List<Diagnostic>();

        }

        private static void NotifyUniqueIdError(
            IMethodSymbol method,
            InvocationExpressionSyntax invocationExpressionSyntax,
            ICollection<Diagnostic> errors)
        {
            errors.Add(Diagnostic.Create(
                DiagnosticsDescriptors.KPropertyExpressionMustHaveUniqueId,
                invocationExpressionSyntax.GetLocation(), method.Name));
        }

        private static void NotifyPropertyAssignmentError(ICollection<Diagnostic> errors, Location location)
            => errors.Add(Diagnostic.Create(DiagnosticsDescriptors.KLambdaAssignmentNotFound, location));

        public void AppendExpression(StringBuilder here)
        {
            const int KEditorFirstLineNumber = 1;
            var location = invocationExpressionSyntax.GetLocation();
            var lineSpan = location.GetLineSpan();
            var uid = lineSpan.Path + (lineSpan.StartLinePosition.Line + KEditorFirstLineNumber).ToString();
            if (uids.Contains(uid))
                NotifyUniqueIdError(method, invocationExpressionSyntax, errors);
            else
            {
                uids.Add(uid);
                var (lambda, parameter) = LambdaLocator.Locate(invocationExpressionSyntax);
                var assignment = AssignmentLocator.Locate(lambda);
                if (lambda != null && parameter != null && assignment != null && lineSpan.IsValid)
                {
                    var leftPart = compilation.GetSemanticModel(assignment.Left.SyntaxTree)
                        .GetSymbolInfo(assignment.Left).Symbol as IPropertySymbol;
                    if (leftPart == null)
                        NotifyPropertyAssignmentError(errors, location);
                    else
                    {
                        var typeSymbol = leftPart.ContainingType;
                        var parameterText = parameter.ToString();
                        here.Append("case \"")
                            .Append(uid)
                            .AppendLine("\":")
                            .Append("return ExpressionUtils.Get<global::")
                            .Append(typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormats.Namespace))
                            .Append(".")
                            .Append(Prefix.PropertyToFuncInterface)
                            .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name, "_", "_"), "_")
                            .Append(typeSymbol.Name)
                            .Append(">(")
                            .Append(parameterText)
                            .Append("=>")
                            .Append(parameterText)
                            .Append(".")
                            .AppendP2FSetter(leftPart)
                            .Append("(")
                            .Append(assignment.Right.ToString())
                            .AppendLine("));");
                        var str = here.ToString();
                    }
                }
                else if (assignment == null)
                {
                    NotifyPropertyAssignmentError(errors, location);
                }
            }
        }

        public IEnumerable<Diagnostic> GetErrors() => errors;

        public IEnumerable<Diagnostic> GetWarnings() => warnings;

        #region locators

        abstract class NodeLocator<T> : CSharpSyntaxWalker
            where T : SyntaxNode
        {
            protected T? result;

            protected NodeLocator() { }

            public override void DefaultVisit(SyntaxNode node)
            {
                if (result == null)
                    base.DefaultVisit(node);
            }
        }


        sealed class LambdaLocator : NodeLocator<LambdaExpressionSyntax>
        {
            ParameterSyntax? parameter;

            public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
            {
                if (node.Parent.IsKind(SyntaxKind.Argument))
                {
                    result = node;
                    parameter = node.ParameterList.Parameters.FirstOrDefault();
                }
                base.VisitParenthesizedLambdaExpression(node);
            }

            public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
            {
                if (node.Parent.IsKind(SyntaxKind.Argument))
                {
                    result = node;
                    parameter = node.Parameter;
                }
                base.VisitSimpleLambdaExpression(node);
            }

            public static (LambdaExpressionSyntax?, ParameterSyntax?) Locate(SyntaxNode? at)
            {
                var @this = new LambdaLocator();
                @this.Visit(at);
                return (@this.result, @this.parameter);
            }
        }

        sealed class AssignmentLocator : NodeLocator<AssignmentExpressionSyntax>
        {
            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                result = node;
                base.VisitAssignmentExpression(node);
            }

            public static AssignmentExpressionSyntax? Locate(SyntaxNode? at)
            {
                var @this = new AssignmentLocator();
                @this.Visit(at);
                return @this.result;
            }
        }

        #endregion
    }
}
