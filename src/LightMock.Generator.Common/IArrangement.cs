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
using System.ComponentModel;
using System.Text;

namespace LightMock
{
    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ICallback : IFluentInterface
    {
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback(Action callback);
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback<T1>(Action<T1> callback);
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback<T1, T2>(Action<T1, T2> callback);
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <typeparam name="T4">Type of fourth parameter of mocked method.</typeparam>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <typeparam name="T4">Type of fourth parameter of mocked method.</typeparam>
        /// <typeparam name="T5">Type of fifth parameter of mocked method.</typeparam>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback);
        /// <summary>
        /// Set a <paramref name="callback"/> that will be invoked when the method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <typeparam name="T4">Type of fourth parameter of mocked method.</typeparam>
        /// <typeparam name="T5">Type of fifth parameter of mocked method.</typeparam>
        /// <typeparam name="T6">Type of sixth parameter of mocked method.</typeparam>
        /// <param name="callback">A callback to invoke.</param>
        /// <returns>Part of fluent interface. <see cref="ICallbackResult"/></returns>
        ICallbackResult Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback);
    }

    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] 
    public interface ICallbackResult : IThrows, IFluentInterface { }

    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IThrows : IFluentInterface
    {
        /// <summary>
        /// Arranges a <typeparamref name="TException"/> which will be thrown when the method is called.
        /// </summary>
        /// <typeparam name="TException">Type of exception</typeparam>
        void Throws<TException>() where TException : Exception, new();
        /// <summary>
        /// Arranges a <typeparamref name="TException"/> which will be thrown when the method is called.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="factory"></param>
        void Throws<TException>(Func<TException> factory) where TException : Exception;

#pragma warning disable CS0419 // Ambiguous reference in cref attribute
        /// <summary>
        /// Cancels previous <see cref="Throws{TException}"/> or <see cref="Throws{TException}(Func{TException})"/> arrangement.
        /// </summary>
        void ThrowsNothing();
#pragma warning restore CS0419 // Ambiguous reference in cref attribute
    }

    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IReturns<TResult> : IFluentInterface
    {
        /// <summary>
        /// Arranges the <paramref name="value"/> to return when method is called.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Part of fluent interface.</returns>
        IReturnsResult<TResult> Returns(TResult value);
        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        IReturnsResult<TResult> Returns(Func<TResult> getResultFunc);
        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        IReturnsResult<TResult> Returns<T1>(Func<T1, TResult> getResultFunc);
        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        IReturnsResult<TResult> Returns<T1, T2>(Func<T1, T2, TResult> getResultFunc);
        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        IReturnsResult<TResult> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> getResultFunc);
        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <typeparam name="T4">Type of fourth parameter of mocked method.</typeparam>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        IReturnsResult<TResult> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> getResultFunc);
    }

    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IReturnsResult<TResult> : ICallback, IThrows, IFluentInterface { }

    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IArrangement : ICallback, IThrows, ICallbackResult, IFluentInterface { }

    /// <summary>
    /// Provides a part of fluent interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IArrangement<TResult> : IReturns<TResult>, IReturnsResult<TResult>, ICallback, IThrows, IFluentInterface { }

}
