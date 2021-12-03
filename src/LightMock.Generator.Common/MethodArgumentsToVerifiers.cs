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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightMock
{
    sealed class MethodArgumentsToVerifiers : ExpressionVisitor
    {
        private readonly MethodInfo method;
        private readonly List<LambdaExpression> lambdas;

        private MethodArgumentsToVerifiers(MethodInfo method)
        {
            this.method = method;
            this.lambdas = new List<LambdaExpression>();
        }

        /// <summary>
        /// Creates list of <see cref="LambdaExpression"/> that can be used to check method arguments.
        /// </summary>
        /// <param name="expression">Method to process</param>
        /// <returns>List of lambda</returns>
        public static IReadOnlyList<LambdaExpression> Convert(MethodCallExpression expression)
        {
            var @this = new MethodArgumentsToVerifiers(expression.Method);
            @this.Visit(expression);
            return @this.lambdas.AsReadOnly();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == method)
            {
                lambdas.AddRange(from argument in node.Arguments
                                 let expression = CreateLambdaExpression(argument)
                                 where expression != null
                                 select expression);
            }
            return base.VisitMethodCall(node);
        }

        LambdaExpression? CreateLambdaExpression(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression constantExpression:
                    return CreateLambdaExpressionForConstant(constantExpression);
                case MethodCallExpression methodCallExpression:
                    return CreateLambdaExpressionForMethodCall(methodCallExpression);
            }
            return null;
        }

        LambdaExpression CreateLambdaExpressionForConstant(ConstantExpression constantExpression)
        {
            var parameterExpression = Expression.Parameter(constantExpression.Type, "p");
            var equalExpression = Expression.Equal(parameterExpression, constantExpression);
            return Expression.Lambda(equalExpression, parameterExpression);
        }

        LambdaExpression CreateLambdaExpressionForMethodCall(MethodCallExpression methodCallExpression)
            => (LambdaExpression)methodCallExpression.Arguments[0];

    }
}
