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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ICallback : IFluentInterface
    {
        ICallbackResult Callback(Action callback);
        ICallbackResult Callback<T>(Action<T> callback);
        ICallbackResult Callback<T1, T2>(Action<T1, T2> callback);
        ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> callback);
        ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback);
        ICallbackResult Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback);
        ICallbackResult Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback);
    }

    [EditorBrowsable(EditorBrowsableState.Never)] 
    public interface ICallbackResult : IThrows, IFluentInterface { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IThrows : IFluentInterface
    {
        void Throws<TException>() where TException : Exception, new();
        void Throws<TException>(Func<TException> factory) where TException : Exception;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IReturns<TResult> : IFluentInterface
    {
        IReturnsResult<TResult> Returns(TResult value);
        IReturnsResult<TResult> Returns(Func<TResult> getResultFunc);
        IReturnsResult<TResult> Returns<T>(Func<T, TResult> getResultFunc);
        IReturnsResult<TResult> Returns<T1, T2>(Func<T1, T2, TResult> getResultFunc);
        IReturnsResult<TResult> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> getResultFunc);
        IReturnsResult<TResult> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> getResultFunc);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IReturnsResult<TResult> : ICallback, IThrows, IFluentInterface { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IArrangement : ICallback, IThrows, ICallbackResult, IFluentInterface { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IArrangement<TResult> : IReturns<TResult>, IReturnsResult<TResult>, ICallback, IThrows, IFluentInterface { }

}
