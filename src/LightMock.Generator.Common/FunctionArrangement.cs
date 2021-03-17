/*****************************************************************************   
    The MIT License (MIT)

    Copyright (c) 2014 bernhard.richter@gmail.com

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
******************************************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace LightMock
{
    /// <summary>
    /// A class that represents an arrangement of a mocked method that 
    /// returns a value of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the mocked method.</typeparam>
    sealed class FunctionArrangement<TResult> : Arrangement, IArrangement<TResult>, IArrangementInvocation<TResult>
    {
        [AllowNull]
        private TResult result;

        private CallbackInvocation callback = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionArrangement{TResult}"/> class.
        /// </summary>
        /// <param name="expression">The <see cref="LambdaExpression"/> that specifies
        /// where to apply this <see cref="Arrangement"/>.</param>
        public FunctionArrangement(LambdaExpression expression)
            : base(expression)
        { }

        FunctionArrangement<TResult> SetCallback(Delegate callback)
        {
            this.callback.Method = callback;
            return this;
        }

        [return: MaybeNull]
        TResult IArrangementInvocation<TResult>.Invoke(IInvocationInfo invocation)
        {
            var exception = GetException();
            var result = invocation.Invoke(callback, this.result);
            InvokeCallback(invocation);
            if (exception != null)
                throw exception;
            return result;
        }

        IReturnsResult<TResult> IReturns<TResult>.Returns(TResult value)
        {
            result = value;
            this.callback.Method = null;
            return this;
        }

        IReturnsResult<TResult> IReturns<TResult>.Returns(Func<TResult> getResultFunc)
            => SetCallback(getResultFunc);

        IReturnsResult<TResult> IReturns<TResult>.Returns<T>(Func<T, TResult> getResultFunc)
            => SetCallback(getResultFunc);

        IReturnsResult<TResult> IReturns<TResult>.Returns<T1, T2>(Func<T1, T2, TResult> getResultFunc)
            => SetCallback(getResultFunc);

        IReturnsResult<TResult> IReturns<TResult>.Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> getResultFunc)
            => SetCallback(getResultFunc);

        IReturnsResult<TResult> IReturns<TResult>.Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> getResultFunc)
            => SetCallback(getResultFunc);
    }
}