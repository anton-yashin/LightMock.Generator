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
using ExpressionReflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightMock
{
    sealed class MethodMatchInfo : IMatchInfo<MethodInvocationInfo>, IMatchInfo, IEquatable<MethodMatchInfo>
    {
        private readonly MethodInfo methodInfo;
        private readonly IReadOnlyList<LambdaExpression> lambdaExpressions;

        public MethodMatchInfo(MethodInfo methodInfo, IReadOnlyList<LambdaExpression> lambdaExpressions)
        {
            this.methodInfo = methodInfo;
            this.lambdaExpressions = lambdaExpressions;
        }

        public bool Equals(IMatchInfo other) => Equals(other as MethodMatchInfo);

        public bool Equals(MethodMatchInfo? other)
        {
            return other != null && methodInfo == other.methodInfo &&
                Equals(lambdaExpressions, other.lambdaExpressions);
        }

        static bool Equals(IReadOnlyCollection<LambdaExpression>? x, IReadOnlyCollection<LambdaExpression>? y)
        {
            if (x == null && y == null)
                return true;
            if (x != null && y != null)
            {
                return x.Count == y.Count
                    && Enumerable.SequenceEqual(x, y, LambdaComparer.Instance);
            }
            return false;
        }

        public bool Matches(MethodInvocationInfo? invocationInfo)
        {
            return invocationInfo != null
                && lambdaExpressions.Count == invocationInfo.Arguments.Length
                && methodInfo == invocationInfo.Method
                && lambdaExpressions.Where((lambda, i)
                => (bool)lambda.Execute(invocationInfo.Arguments[i]) == false).Any() == false;
        }

        public bool Matches(IInvocationInfo? invocationInfo)
            => Matches(invocationInfo as MethodInvocationInfo);
    }
}
