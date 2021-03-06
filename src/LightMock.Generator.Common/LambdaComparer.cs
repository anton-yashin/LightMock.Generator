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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace LightMock
{
    internal class LambdaComparer : IEqualityComparer<Expression>
    {
        static LambdaComparer? instance;
        public static LambdaComparer Instance => LazyInitializer.EnsureInitialized(ref instance!);

        public bool Equals(Expression x, Expression y)
        {
            if (object.ReferenceEquals(x, y))
                return true;
            if (x == null || y == null)
                return false;
            if (x.NodeType == y.NodeType && x.Type == y.Type)
            {
                switch (x)
                {
                    case LambdaExpression lambda:
                        return EqualsLambda(lambda, y);
                    case MethodCallExpression methodCall:
                        return EqualsMethodCall(methodCall, y);
                    case ParameterExpression _:
                        return true;
                    case ConstantExpression constant:
                        return EqualsConstant(constant, y);
                    case MemberExpression member:
                        return EqualsMember(member, y);
                    case BinaryExpression binary:
                        return EqualsBinary(binary, y);
                }
                throw new InvalidOperationException($"expression type {x.NodeType}/{x.GetType()} is not supported");
            }
            return false;
        }

        private bool EqualsBinary(BinaryExpression left, Expression y)
        {
            var right = (BinaryExpression)y;
            return left.Method == right.Method
                && Equals(left.Left, right.Left)
                && Equals(left.Right, right.Right)
                && Equals(left.Conversion, right.Conversion);
        }

        private bool EqualsMember(MemberExpression left, Expression y)
        {
            var right = (MemberExpression)y;
            return left.Member == right.Member
                && Equals(left.Expression, right.Expression);
        }

        private bool EqualsConstant(ConstantExpression left, Expression y)
        {
            var right = (ConstantExpression)y;
            return object.Equals(left.Value, right.Value);
        }

        private bool EqualsMethodCall(MethodCallExpression left, Expression y)
        {
            var right = (MethodCallExpression)y;
            return left.Method == right.Method
                && Equals(left.Object, right.Object)
                && EqualsParameters(left.Arguments, right.Arguments);
        }

        private bool EqualsLambda(LambdaExpression left, Expression y)
        {
            var right = (LambdaExpression)y;
            return Equals(left.Body, right.Body) && EqualsParameters(left.Parameters, right.Parameters);
        }

        private bool EqualsParameters<T>(ReadOnlyCollection<T> left, ReadOnlyCollection<T> right)
            where T : Expression
        {
            return left.Count == right.Count && Enumerable.SequenceEqual(left, right, this);
        }

        public int GetHashCode(Expression obj) => obj?.GetHashCode() ?? 0;

    }
}
