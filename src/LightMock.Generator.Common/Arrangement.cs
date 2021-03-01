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
        private Action throwAction;
        private Action<object[]?> callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="Arrangement"/> class.
        /// </summary>
        /// <param name="expression">The <see cref="LambdaExpression"/> that specifies
        /// where to apply this <see cref="Arrangement"/>.</param>
        protected Arrangement(LambdaExpression expression)
        {
            this.expression = expression;
            throwAction = () => { };
            callback = (args) => { };
        }

        /// <summary>
        /// Arranges for an <see cref="Exception"/> of type <typeparamref name="TException"/> to be thrown.
        /// </summary>
        /// <typeparam name="TException">The type of <see cref="Exception"/> to be thrown.</typeparam>
        public void Throws<TException>() where TException : Exception, new()
        {
            throwAction = () => { throw new TException(); };
        }

        /// <summary>
        /// Arranges for an <see cref="Exception"/> of type <typeparamref name="TException"/> to be thrown.
        /// </summary>
        /// <typeparam name="TException">The type of <see cref="Exception"/> to be thrown.</typeparam>
        /// <param name="factory">A factory delegate used to create the <typeparamref name="TException"/> instance.</param>
        public void Throws<TException>(Func<TException> factory) where TException : Exception
        {
            throwAction = () => { throw factory(); };
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <param name="callBack">The <see cref="Action"/> to be called when the mocked method is invoked.</param>
        public void Callback(Action callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <typeparam name="T">The type of the first parameter.</typeparam>
        /// <param name="callBack">The <see cref="Action{T}"/> to be called when the mocked method is invoked.</param>
        public void Callback<T>(Action<T> callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <param name="callBack">The <see cref="Action{T1,T2}"/> to be called when the mocked method is invoked.</param>
        public void Callback<T1, T2>(Action<T1, T2> callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <param name="callBack">The <see cref="Action{T1,T2}"/> to be called when the mocked method is invoked.</param>
        public void Callback<T1, T2, T3>(Action<T1, T2, T3> callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <param name="callBack">The <see cref="Action{T1,T2}"/> to be called when the mocked method is invoked.</param>
        public void Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
        /// <param name="callBack">The <see cref="Action{T1,T2}"/> to be called when the mocked method is invoked.</param>
        public void Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Arranges for the <paramref name="callBack"/> to be called when the mocked method is invoked.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter.</typeparam>
        /// <param name="callBack">The <see cref="Action{T1,T2}"/> to be called when the mocked method is invoked.</param>
        public void Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callBack)
        {
            callback = args => callBack.DynamicInvoke(args);
        }

        /// <summary>
        /// Determines if the <paramref name="invocationInfo"/> matches this <see cref="Arrangement"/>.
        /// </summary>
        /// <param name="invocationInfo">The <see cref="InvocationInfo"/> that represents the method invocation.</param>
        /// <returns><b>True</b> if the <paramref name="invocationInfo"/> matches this <see cref="Arrangement"/>, otherwise, <b>False</b>.</returns>
        public bool Matches(IInvocationInfo invocationInfo)
        {
            return expression.ToMatchInfo().Matches(invocationInfo);
        }

        /// <summary>
        /// Determines if the <paramref name="matchInfo"/> matches this <see cref="Arrangement"/>.
        /// </summary>
        /// <param name="matchInfo">The <see cref="MatchInfo"/> that represents the method invocation.</param>
        /// <returns><b>True</b> if the <paramref name="matchInfo"/> matches this <see cref="Arrangement"/>, otherwise, <b>False</b>.</returns>
        public bool Matches(IMatchInfo matchInfo)
        {
            return expression.ToMatchInfo().Equals(matchInfo);
        }

        protected void InvokeThrowAction() => throwAction();

        protected void InvokeCallback(IInvocationInfo invocationInfo)
            => invocationInfo.Invoke(callback);
    }
}