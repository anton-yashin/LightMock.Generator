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
using System.Text;

namespace LightMock
{
    /// <summary>
    /// This class designed to extract values of <see cref="ConstantExpression"/> from <see cref="LambdaExpression"/>
    /// </summary>
    sealed class ConstantExpressionValuesLocator : ExpressionVisitor
    {
        List<object> values = new List<object>();

        private ConstantExpressionValuesLocator() { }

        /// <summary>
        /// Extract values of <see cref="ConstantExpression"/>
        /// </summary>
        /// <param name="at">The <see cref="LambdaExpression"/> from which to extract <see cref="ConstantExpression.Value"/>'s</param>
        /// <returns>Extracted values</returns>
        public static object[] Locate(LambdaExpression at)
        {
            var @this = new ConstantExpressionValuesLocator();
            @this.Visit(at);
            return @this.values.ToArray();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            values.AddRange(from i in node.Arguments
                               where i.NodeType == ExpressionType.Constant
                               select ((ConstantExpression)i).Value);

            return base.VisitMethodCall(node);
        }
    }
}
