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
using System.Linq.Expressions;

namespace LightMock
{
    /// <summary>
    /// A class that represents an arrangement of a mocked method.
    /// </summary>
    abstract class Arrangement : IArrangement
    {
        private readonly LambdaExpression expression;                
        private readonly CallbackInvocation exceptionFactory = new();
        private readonly CallbackInvocation callback = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Arrangement"/> class.
        /// </summary>
        /// <param name="expression">The <see cref="LambdaExpression"/> that specifies
        /// where to apply this <see cref="Arrangement"/>.</param>
        protected Arrangement(LambdaExpression expression)
        {
            this.expression = expression;
        }

        Arrangement Throws<TException>() where TException : Exception, new()
            => Throws(() => new TException());

        Arrangement Throws<TException>(Func<TException> factory) where TException : Exception
        {
            exceptionFactory.Method = factory;
            return this;
        }

        Arrangement Callback(Action callback)
        {
            this.callback.Method = callback;
            return this;
        }

        Arrangement Callback<T>(Action<T> callback)
        {
            this.callback.Method = callback;
            return this;
        }

        Arrangement Callback<T1, T2>(Action<T1, T2> callback)
        {
            this.callback.Method = callback;
            return this;
        }

        Arrangement Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
        {
            this.callback.Method = callback;
            return this;
        }

        Arrangement Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
        {
            this.callback.Method = callback;
            return this;
        }

        Arrangement Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
        {
            this.callback.Method = callback;
            return this;
        }

        Arrangement Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
        {
            this.callback.Method = callback;
            return this;
        }

        /// <summary>
        /// Determines if the <paramref name="invocationInfo"/> matches this <see cref="Arrangement"/>.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="IInvocationInfo"/> that represents the method invocation.</param>
        /// <returns><b>True</b> if the <paramref name="invocationInfo"/> matches this <see cref="Arrangement"/>, otherwise, <b>False</b>.</returns>
        public bool Matches(IInvocationInfo invocationInfo)
        {
            return expression.ToMatchInfo().Matches(invocationInfo);
        }

        /// <summary>
        /// Determines if the <paramref name="matchInfo"/> matches this <see cref="Arrangement"/>.
        /// </summary>
        /// <param name="matchInfo">The <see cref="IMatchInfo"/> that represents the method invocation.</param>
        /// <returns><b>True</b> if the <paramref name="matchInfo"/> matches this <see cref="Arrangement"/>, otherwise, <b>False</b>.</returns>
        public bool Matches(IMatchInfo matchInfo)
        {
            return expression.ToMatchInfo().Equals(matchInfo);
        }


        protected Exception? GetException() => exceptionFactory.Invoke<Exception>(null);

        protected void InvokeCallback(IInvocationInfo invocationInfo)
            => invocationInfo.Invoke(callback);

        ICallbackResult ICallback.Callback(Action callback)
            => Callback(callback);

        ICallbackResult ICallback.Callback<T>(Action<T> callback)
            => Callback(callback);

        ICallbackResult ICallback.Callback<T1, T2>(Action<T1, T2> callback)
            => Callback(callback);

        ICallbackResult ICallback.Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
            => Callback(callback);

        ICallbackResult ICallback.Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
            => Callback(callback);

        ICallbackResult ICallback.Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
            => Callback(callback);

        ICallbackResult ICallback.Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
            => Callback(callback);

        void IThrows.Throws<TException>()
            => Throws<TException>();

        void IThrows.Throws<TException>(Func<TException> factory)
            => Throws(factory);
    }
}