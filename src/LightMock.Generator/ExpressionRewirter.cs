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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
            var location = invocationExpressionSyntax.ArgumentList.GetLocation();
            var lineSpan = location.GetLineSpan();
            var al = invocationExpressionSyntax.ArgumentList;
            var (lambdaSyntax, syntaxUidPart1, syntaxUidPart2) =
                (al.GetArgument("expression", 0),
                al.GetArgument("uidPart1", 1),
                al.GetArgument("uidPart2", 2));

            LiteralExpressionSyntax? le;
            var uidPart1 = syntaxUidPart1 == null || (le = LiteralExpressionLocator.Locate(syntaxUidPart1)) == null
                ? lineSpan.Path
                : le.ToString()
                    // unescape string
                    .Replace("\"", "")
                    .Replace(@"\\", @"\");

            var uidPart2 = syntaxUidPart2 == null || (le = LiteralExpressionLocator.Locate(syntaxUidPart2)) == null
                ? (lineSpan.StartLinePosition.Line + KEditorFirstLineNumber).ToString()
                : le.ToString();

            var uid = uidPart1 + uidPart2;
            if (uids.Contains(uid))
            {
                NotifyUniqueIdError(method, invocationExpressionSyntax, errors);
            }
            else
            {
                var (lambda, parameter) = LambdaLocator.Locate(lambdaSyntax);
                var assignment = AssignmentLocator.Locate(lambda);
                if (lambda != null && parameter != null && assignment != null && lineSpan.IsValid)
                {
                    var leftPart = compilation.GetSemanticModel(assignment.Left.SyntaxTree)
                        .GetSymbolInfo(assignment.Left).Symbol as IPropertySymbol;
                    if (leftPart == null)
                    {
                        NotifyPropertyAssignmentError(errors, location);
                    }
                    else
                    {
                        var typeSymbol = leftPart.ContainingType;
                        var parameterText = parameter.ToString();
                        here.Append("case \"")
                            .Append(uid.Replace(@"\", @"\\"))
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
                        uids.Add(uid);
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

            public static T? Locate<TLocator>(SyntaxNode? node)
                where TLocator : NodeLocator<T>, new()
            {
                var @this = new TLocator();
                @this.Visit(node);
                return @this.result;
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
                => Locate<AssignmentLocator>(at);
        }

        sealed class LiteralExpressionLocator : NodeLocator<LiteralExpressionSyntax>
        {
            public override void VisitLiteralExpression(LiteralExpressionSyntax node)
            {
                result = node;
                base.VisitLiteralExpression(node);
            }

            public static LiteralExpressionSyntax? Locate(SyntaxNode? at)
                => Locate<LiteralExpressionLocator>(at);
        }

        #endregion
    }
}
