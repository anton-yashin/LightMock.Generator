﻿/******************************************************************************
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

namespace LightMock.Generator
{
    sealed class ArrangeExpressionRewriter : ExpressionRewriter
    {
        public ArrangeExpressionRewriter(
            IMethodSymbol method,
            InvocationExpressionSyntax invocationExpressionSyntax,
            CSharpCompilation compilation)
            : base(method, invocationExpressionSyntax, compilation)
        { }

        protected override ArgumentSyntax? GetLambda(ArgumentListSyntax argumentList)
            => argumentList.GetArgument("expression", 0);

        protected override ArgumentSyntax? GetUidPart1(ArgumentListSyntax argumentList)
            => argumentList.GetArgument("uidPart1", 1);

        protected override ArgumentSyntax? GetUidPart2(ArgumentListSyntax argumentList)
            => argumentList.GetArgument("uidPart2", 2);
    }
}
