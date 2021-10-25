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
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LightMock.Generator
{
    abstract class ExpressionRewriter
    {
        protected readonly IMethodSymbol method;
        private readonly InvocationExpressionSyntax invocationExpressionSyntax;
        private readonly CSharpCompilation compilation;
        private readonly ICollection<string> uids;
        private readonly Location location;
        private readonly string className;
        private readonly string uid;
        private readonly ParameterSyntax? parameter;
        private readonly AssignmentExpressionSyntax? assignment;
        private readonly IPropertySymbol? leftPart;

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

            const int KEditorFirstLineNumber = 1;
            location = invocationExpressionSyntax.ArgumentList.GetLocation();
            var lineSpan = location.GetLineSpan();
            var al = invocationExpressionSyntax.ArgumentList;
            var (lambdaSyntax, syntaxUidPart1, syntaxUidPart2) = (GetLambda(al), GetUidPart1(al), GetUidPart2(al));

            LiteralExpressionSyntax ? le;
            var uidPart1 = syntaxUidPart1 == null || (le = LiteralExpressionLocator.Locate(syntaxUidPart1)) == null
                ? Path.GetFullPath(lineSpan.Path)
                : le.ToString()
                    // unescape string
                    .Replace("\"", "")
                    .Replace(@"\\", @"\");

            var uidPart2 = syntaxUidPart2 == null || (le = LiteralExpressionLocator.Locate(syntaxUidPart2)) == null
                ? (lineSpan.StartLinePosition.Line + KEditorFirstLineNumber).ToString()
                : le.ToString();

            using (var hash = SHA256.Create())
                className = '_' + Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(uidPart1)))
                    .Replace('+', 'ф').Replace('=', 'ы').Replace('/', 'п') + "_" + uidPart2;

            uid = uidPart2 + uidPart1;
            var (lambda, parameter) = LambdaLocator.Locate(lambdaSyntax);
            this.parameter = parameter;
            assignment = AssignmentLocator.Locate(lambda);

            if (assignment != null)
            {
                leftPart = compilation.GetSemanticModel(assignment.Left.SyntaxTree)
                    .GetSymbolInfo(assignment.Left).Symbol as IPropertySymbol;
            }
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

        public SourceText DoGenerate()
        {
            if (leftPart == null || parameter == null || assignment == null)
                throw new InvalidOperationException();
            var typeSymbol = leftPart.ContainingType;
            var parameterText = parameter.ToString();

            var code = new StringBuilder(@"// <auto-generated />
namespace LightMock.Generator.Tokens
{
    sealed class ").Append(className).Append(@" : global::LightMock.Generator.").Append(nameof(ILambdaToken)).Append(@"
    {
        public string Key => @""").Append(uid).Append(@""";
        public global::System.Linq.Expressions.LambdaExpression Value
            => ExpressionUtils.Get<global::").Append(typeSymbol.ContainingNamespace, SymbolDisplayFormats.Namespace)
            .Append("." + Prefix.PropertyToFuncInterface)
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
            .AppendLine("));")
            .Append(@"
    }
}
");
            var text = code.ToString();
            return SourceText.From(code.ToString(), Encoding.UTF8);
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


        public IEnumerable<Diagnostic> GetErrors()
        {
            List<Diagnostic> errors = new List<Diagnostic>();
            if (uids.Contains(uid))
            {
                NotifyUniqueIdError(method, invocationExpressionSyntax, errors);
            }
            if (leftPart == null)
            {
                NotifyPropertyAssignmentError(errors, location);
            }
            if (assignment == null)
            {
                NotifyPropertyAssignmentError(errors, location);
            }
            return errors;
        }

        public IEnumerable<Diagnostic> GetWarnings() => Enumerable.Empty<Diagnostic>();
        public string FileName => className + Suffix.FileName;

        protected abstract ArgumentSyntax? GetLambda(ArgumentListSyntax argumentList);
        protected abstract ArgumentSyntax? GetUidPart1(ArgumentListSyntax argumentList);
        protected abstract ArgumentSyntax? GetUidPart2(ArgumentListSyntax argumentList);
    }
}