﻿/*****************************************************************************   
    The MIT License (MIT)

    Copyright (c) 2014 bernhard.richter@gmail.com
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
******************************************************************************    
    https://github.com/seesharper/LightMock
    http://twitter.com/bernhardrichter
    https://github.com/anton-yashin/
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace LightMock
{
    /// <summary>
    /// A class that represents the mock context for a given <typeparamref name="TMock"/> type.
    /// </summary>
    /// <typeparam name="TMock">The target mock type.</typeparam>
    sealed class MockContext<TMock> : IMockContext<TMock>, IInvocationContext<TMock>, IMockContextInternal
    {
        private readonly ILockedCollection<IInvocationInfo> invocations = new LockedCollection<IInvocationInfo>();
        private readonly ILockedCollection<Arrangement> arrangements = new LockedCollection<Arrangement>();
        private readonly ILockedCollection<IInvocationInfo> verifiedInvocations = new LockedCollection<IInvocationInfo>();

        /// <summary>
        /// Arranges a mocked method. 
        /// </summary>
        /// <param name="matchExpression">The match expression that describes where 
        /// this <see cref="IArrangement"/> will be applied.</param>
        /// <returns>A new <see cref="IArrangement"/> used to apply method behavior.</returns>
        public IArrangement Arrange(Expression<Action<TMock>> matchExpression)
            => ArrangeAction(matchExpression);

        IArrangement ArrangeAction(LambdaExpression matchExpression)
        {
            var matchInfo = matchExpression.ToMatchInfo();
            var arrangement = arrangements.InvokeLocked(
                c => (from i in c
                      let a = i as ActionArrangement
                      where a != null && a.Matches(matchInfo)
                      select a).FirstOrDefault());
            if (arrangement == null)
            {
                arrangement = new ActionArrangement(matchExpression);
                arrangements.Add(arrangement);
            }
            return arrangement;
        }

        /// <summary>
        /// Arranges a mocked method. 
        /// </summary>
        /// <typeparam name="TResult">The type of value returned from the mocked method.</typeparam>
        /// <param name="matchExpression">The match expression that describes where 
        /// this <see cref="IArrangement{TResult}"/> will be applied.</param>
        /// <returns>A new <see cref="IArrangement{TResult}"/> used to apply method behavior.</returns>
        public IArrangement<TResult> Arrange<TResult>(Expression<Func<TMock, TResult>> matchExpression)
            => ArrangeFunction<TResult>(matchExpression);

        IArrangement<TResult> ArrangeFunction<TResult>(LambdaExpression matchExpression)
        {
            var matchInfo = matchExpression.ToMatchInfo();
            var arrangement = arrangements.InvokeLocked(
                c => (from i in c
                     let a = i as FunctionArrangement<TResult>
                     where a != null && a.Matches(matchInfo)
                     select a).FirstOrDefault());
            if (arrangement == null)
            {
                arrangement = new FunctionArrangement<TResult>(matchExpression);
                arrangements.Add(arrangement);
            }
            return arrangement;
        }

        /// <summary>
        /// Arranges a mocked property. 
        /// </summary>
        /// <typeparam name="TResult">The type of value returned from the mocked property.</typeparam>
        /// <param name="matchExpression">The match expression that describes where 
        /// this <see cref="IArrangement{TResult}"/> will be applied.</param>
        /// <returns>A new <see cref="IArrangement{TResult}"/> used to apply property behavior.</returns>
        public IArrangement ArrangeProperty<TResult>(Expression<Func<TMock, TResult>> matchExpression)
            => ArrangePropertyInternal<TResult>(matchExpression);

        public IArrangement ArrangePropertyInternal<TProperty>(LambdaExpression matchExpression)
        {
            var matchInfo = matchExpression.ToMatchInfo();
            var arrangement = arrangements.InvokeLocked(
                c => (from i in arrangements
                      let a = i as PropertyArrangement<TProperty>
                      where a != null && a.Matches(matchInfo)
                      select a).FirstOrDefault());
            if (arrangement == null)
            {
                arrangement = new PropertyArrangement<TProperty>(matchExpression);
                arrangements.Add(arrangement);
            }
            return arrangement;
        }

        /// <summary>
        /// Verifies that the method represented by the <paramref name="matchExpression"/> has 
        /// been invoked.
        /// </summary>
        /// <param name="matchExpression">The <see cref="Expression{TDelegate}"/> that represents 
        /// the method invocation to be verified.</param>
        public void Assert(Expression<Action<TMock>> matchExpression)
        {
            Assert(matchExpression, Invoked.AtLeast(1));            
        }

        /// <summary>
        /// Verifies that the method represented by the <paramref name="matchExpression"/> has 
        /// been invoked the specified number of <paramref name="invoked"/>.
        /// </summary>
        /// <param name="matchExpression">The <see cref="Expression{TDelegate}"/> that represents 
        /// the method invocation to be verified.</param>
        /// <param name="invoked">Specifies the number of times we expect the mocked method to be invoked.</param>
        public void Assert(Expression<Action<TMock>> matchExpression, Invoked invoked)
        {
            AssertInternal(matchExpression, invoked);
        }

        public void AssertInternal(LambdaExpression matchExpression, Invoked invoked)
        {
            var matchInfo = matchExpression.ToMatchInfo();
            var invocations = this.invocations.InvokeLocked(c => c.Where(matchInfo.Matches).ToArray());

            if (invoked.Verify(invocations.Length))
                verifiedInvocations.AddRange(invocations);
            else
                throw new MockException(string.Format("The method {0} was called {1} times", matchExpression.Simplify(), invocations.Length));
        }

        /// <inheritdoc/>
        void IInvocationContext<TMock>.Invoke(Expression<Action<TMock>> expression)
            => Invoke(expression, null);

        /// <inheritdoc/>
        void IInvocationContext<TMock>.Invoke(
            Expression<Action<TMock>> expression,
            IDictionary<string, object>? refValues)
        { 
            Invoke(expression, refValues);
        }

        void Invoke(
            Expression<Action<TMock>> expression,
            IDictionary<string, object>? refValues)
        {
            var invocationInfo = expression.ToInvocationInfo();
            invocations.Add(invocationInfo);

            var arrangement = arrangements.InvokeLocked(
                c => (from i in arrangements
                      let a = i as IArrangementInvocation
                      where a != null && a.Matches(invocationInfo)
                      select a).FirstOrDefault());

            if (arrangement != null)
            {
                arrangement.Invoke(invocationInfo, refValues);
            }            
        }

        /// <inheritdoc/>
        [return: MaybeNull]
        TResult IInvocationContext<TMock>.Invoke<TResult>(
            Expression<Func<TMock, TResult>> expression)
        { 
            return Invoke<TResult>(expression, null);
        }

        /// <inheritdoc/>
        [return: MaybeNull]
        TResult IInvocationContext<TMock>.Invoke<TResult>(
            Expression<Func<TMock, TResult>> expression,
            IDictionary<string, object>? refValues)
        {
            return Invoke<TResult>(expression, refValues);
        }

        /// <inheritdoc/>
        [return: MaybeNull]
        TResult Invoke<TResult>(
            Expression<Func<TMock, TResult>> expression,
            IDictionary<string, object>? refValues)
        {
            var invocationInfo = expression.ToInvocationInfo();
            invocations.Add(invocationInfo);

            var arrangement = arrangements.InvokeLocked(
                c => (from i in c
                      let a = i as IArrangementInvocation<TResult>
                      where a != null && a.Matches(invocationInfo)
                      select a).FirstOrDefault());
            if (arrangement != null)
            {
                return arrangement.Invoke(invocationInfo, refValues);
            }

            return default(TResult);
        }

        /// <summary>
        /// Tracks that the setter represented by the <paramref name="expression"/>
        /// has been invoked.
        /// </summary>
        /// <param name="expression">The <see cref="Expression{TDelegate}"/> that 
        /// represents the setter that has been invoked.</param>
        /// <param name="value">The value</param>
        void IInvocationContext<TMock>.InvokeSetter<TResult>(Expression<Func<TMock, TResult>> expression, TResult value)
        {
            var invocationInfo = expression.ToInvocationInfo();
            invocations.Add(invocationInfo);

            var arrangement = arrangements.InvokeLocked(
                c => (from i in arrangements
                      let a = i as IPropertyArrangementInvocation<TResult>
                      where a != null && a.Matches(invocationInfo)
                      select a).FirstOrDefault());

            if (arrangement != null)
            {
                arrangement.Invoke(value);
            }
        }

        #region IMockContextInternal implementation

        IArrangement IMockContextInternal.ArrangeAction(LambdaExpression matchExpression) 
            => ArrangeAction(matchExpression);
        IArrangement<TResult> IMockContextInternal.ArrangeFunction<TResult>(LambdaExpression matchExpression)
            => ArrangeFunction<TResult>(matchExpression);
        IArrangement IMockContextInternal.ArrangeProperty<TProperty>(LambdaExpression matchExpression)
            => ArrangePropertyInternal<TProperty>(matchExpression);

        public IEnumerable<IInvocationInfo> GetUnverifiedInvocations()
        {
            var verified = verifiedInvocations.ToArray();
            var invocations = this.invocations.ToArray();
            foreach (var i in invocations)
            {
                bool found = false;
                foreach (var j in verified)
                {
                    if (ReferenceEquals(i, j))
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                    yield return i;
            }
        }

        #endregion
    }
}