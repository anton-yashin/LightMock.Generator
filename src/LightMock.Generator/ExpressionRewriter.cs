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
using LightMock.Generator.Locators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LightMock.Generator
{
    abstract class ExpressionRewriter
    {
        protected readonly IMethodSymbol method;
        private readonly InvocationExpressionSyntax invocationExpressionSyntax;
        private readonly CSharpCompilation compilation;
        private readonly ICollection<string> uids;
        private readonly ICollection<Diagnostic> errors;
        private readonly ICollection<Diagnostic> warnings;

        protected ExpressionRewriter(
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
            var (lambdaSyntax, syntaxUidPart1, syntaxUidPart2) = (GetLambda(al), GetUidPart1(al), GetUidPart2(al));

            LiteralExpressionSyntax? le;
            var uidPart1 = syntaxUidPart1 == null || (le = LiteralExpressionLocator.Locate(syntaxUidPart1)) == null
                ? Path.GetFullPath(lineSpan.Path)
                : (le.ToString()
                    // unescape string
                    .Replace("\"", "")
                    .Replace(@"\\", @"\"));

            var uidPart2 = syntaxUidPart2 == null || (le = LiteralExpressionLocator.Locate(syntaxUidPart2)) == null
                ? (lineSpan.StartLinePosition.Line + KEditorFirstLineNumber).ToString()
                : le.ToString();

            var uid = uidPart2 + uidPart1;
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
                            .Append(typeSymbol.ContainingNamespace, SymbolDisplayFormats.Namespace)
                            .Append(".")
                            .Append(Prefix.PropertyToFuncInterface)
                            .AppendContainingTypes<string>(typeSymbol, (sb, ts) => sb.AppendTypeArguments(ts, i => i.Name, "_", "_"), "_")
                            .Append(typeSymbol.Name.Replace(Prefix.ProtectedToPublicInterface, ""))
                            .Append(">(")
                            .Append(parameterText)
                            .Append("=>")
                            .Append(parameterText)
                            .Append(".")
                            .AppendP2FName(leftPart, Suffix.Setter, Mutator)
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

        SymbolDisplayPart Mutator(SymbolDisplayPart part)
        {
            switch (part.Kind)
            {
                case SymbolDisplayPartKind.Punctuation when part.ToString() == ".":
                    return new SymbolDisplayPart(part.Kind, part.Symbol, "_");
                case SymbolDisplayPartKind.InterfaceName when part.ToString().StartsWith(Prefix.ProtectedToPublicInterface):
                    return new SymbolDisplayPart(part.Kind, part.Symbol,
                        part.ToString().Replace(Prefix.ProtectedToPublicInterface, ""));
            }

            return part;
        }


        public IEnumerable<Diagnostic> GetErrors() => errors;

        public IEnumerable<Diagnostic> GetWarnings() => warnings;

        protected abstract ArgumentSyntax? GetLambda(ArgumentListSyntax argumentList);
        protected abstract ArgumentSyntax? GetUidPart1(ArgumentListSyntax argumentList);
        protected abstract ArgumentSyntax? GetUidPart2(ArgumentListSyntax argumentList);
    }
}