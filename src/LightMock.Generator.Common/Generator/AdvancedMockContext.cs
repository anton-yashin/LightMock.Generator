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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace LightMock.Generator
{
    sealed class AdvancedMockContext<T> : IAdvancedMockContext<T>, IInvocationContext<T>, IAdvancedMockContext
    {
        private readonly object[] @params;
        private readonly TypeResolver typeResolver;
        private readonly Func<string, LambdaExpression> exchangeForExpression;
        private readonly MockContext<T> mockContext;
        private readonly IMockContextInternal propertiesContext;

        public AdvancedMockContext(object[] @params, TypeResolver typeResolver, Func<string, LambdaExpression> exchangeForExpression)
        {
            this.@params = @params;
            this.typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));
            this.exchangeForExpression = exchangeForExpression ?? throw new ArgumentNullException(nameof(exchangeForExpression));
            this.mockContext = new MockContext<T>();
            this.propertiesContext = typeResolver.GetPropertiesContext<T>();
        }

        IInvocationContext<T> InvocationContext => mockContext;

        object[] GetAssertArgs(Invoked invoked)
        {
            const int offset = 2;
            var args = new object[@params.Length + offset];
            args[0] = propertiesContext;
            args[1] = invoked;
            Array.Copy(@params, 0, args, offset, @params.Length);
            return args;
        }

        object[] GetArrangeArgs(ILambdaRequest request)
        {
            const int offset = 1;
            var args = new object[@params.Length + offset];
            args[0] = request;
            Array.Copy(@params, 0, args, offset, @params.Length);
            return args;
        }

        T CreateAssertWhenInstance(Invoked invoked)
            => typeResolver.ActivateAssertWhenInstance<T>(GetAssertArgs(invoked));

        T CreateAssertWhenAnyInstance(Invoked invoked)
            => typeResolver.ActivateAssertWhenAnyInstance<T>(GetAssertArgs(invoked));

        T CreateArrangeWhenAnyInstance(ILambdaRequest request)
            => typeResolver.ActivateArrangeWhenAnyInstance<T>(GetArrangeArgs(request));

        T CreateArrangeWhenInstance(ILambdaRequest request)
            => typeResolver.ActivateArrangeWhenInstance<T>(GetArrangeArgs(request));

        IArrangement Arrange_NoAot(Action<T> expression, Func<ILambdaRequest, T> instanceFactory)
        {
            var request = new LambdaRequest();
            expression(instanceFactory(request));
            var result = request.Result ?? throw new InvalidOperationException("A property assignment is required.");
            return propertiesContext.ArrangeAction(result);
        }

        const string KUidExceptionMessage = "you must provide part of unique identifier";

        public IArrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            return propertiesContext.ArrangeAction(exchangeForExpression(uidPart2 + uidPart1));
        }

        public IArrangement ArrangeSetter_When(Action<T> expression)
            => Arrange_NoAot(expression, CreateArrangeWhenInstance);

        public IArrangement ArrangeSetter_WhenAny(Action<T> expression)
            => Arrange_NoAot(expression, CreateArrangeWhenAnyInstance);

        public IArrangement ArrangeAdd_When(Action<T> expression)
            => Arrange_NoAot(expression, CreateArrangeWhenInstance);

        public IArrangement ArrangeAdd_WhenAny(Action<T> expression)
            => Arrange_NoAot(expression, CreateArrangeWhenAnyInstance);

        public IArrangement ArrangeRemove_When(Action<T> expression)
            => Arrange_NoAot(expression, CreateArrangeWhenInstance);

        public IArrangement ArrangeRemove_WhenAny(Action<T> expression)
            => Arrange_NoAot(expression, CreateArrangeWhenAnyInstance);

        void AssertUsingAssertInstance(Action<T> expression, Invoked times)
            => expression(CreateAssertWhenInstance(times));

        void AssertUsingAssertWhenAnyInstance(Action<T> expression, Invoked times)
            => expression(CreateAssertWhenAnyInstance(times));

        public void AssertAdd_When(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertAdd_When(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertAdd_WhenAny(Action<T> expression, Invoked times)
            => AssertUsingAssertWhenAnyInstance(expression, times);

        public void AssertAdd_WhenAny(Action<T> expression)
            => AssertUsingAssertWhenAnyInstance(expression, Invoked.AtLeast(1));

        public void AssertGet<TProperty>(Func<T, TProperty> expression)
            => AssertGet(expression, Invoked.AtLeast(1));

        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
            => expression(CreateAssertWhenInstance(times));

        public void AssertRemove_When(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertRemove_When(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertRemove_WhenAny(Action<T> expression, Invoked times)
            => AssertUsingAssertWhenAnyInstance(expression, times);

        public void AssertRemove_WhenAny(Action<T> expression)
            => AssertUsingAssertWhenAnyInstance(expression, Invoked.AtLeast(1));

        public void AssertSet(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => AssertSet(expression, Invoked.AtLeast(1), uidPart1, uidPart2);

        public void AssertSet(Action<T> expression, Invoked times, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            propertiesContext.AssertInternal(exchangeForExpression(uidPart2 + uidPart1), times);
        }

        public void AssertSet_When(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertSet_When(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertSet_WhenAny(Action<T> propertySelector)
            => AssertSet_WhenAny(propertySelector, Invoked.AtLeast(1));

        public void AssertSet_WhenAny(Action<T> propertySelector, Invoked times)
            => AssertUsingAssertWhenAnyInstance(propertySelector, times);

        public IEnumerable<IInvocationInfo> GetUnverifiedInvocations()
            => mockContext.GetUnverifiedInvocations();

        #region IMockContext<T> implementation

        public IArrangement Arrange(Expression<Action<T>> matchExpression)
            => mockContext.Arrange(matchExpression);

        public IArrangement<TResult> Arrange<TResult>(Expression<Func<T, TResult>> matchExpression)
            => mockContext.Arrange(matchExpression);

        public IArrangement ArrangeProperty<TResult>(Expression<Func<T, TResult>> matchExpression)
            => mockContext.ArrangeProperty(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression)
            => mockContext.Assert(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression, Invoked invoked)
            => mockContext.Assert(matchExpression, invoked);

        #endregion

        #region IInvocationContext<T> implementation

        void IInvocationContext<T>.Invoke(Expression<Action<T>> expression)
            => InvocationContext.Invoke(expression);

        [return: MaybeNull]
        TResult IInvocationContext<T>.Invoke<TResult>(Expression<Func<T, TResult>> expression)
            => InvocationContext.Invoke(expression);

        void IInvocationContext<T>.InvokeSetter<TResult>(Expression<Func<T, TResult>> expression, TResult value)
            => InvocationContext.InvokeSetter(expression, value);

        #endregion
    }
}
