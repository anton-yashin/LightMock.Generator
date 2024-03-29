﻿/*****************************************************************************   
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace LightMock
{
    /// <summary>
    /// Represents a class that keeps track of method invocations made to 
    /// a manual mock object. 
    /// </summary>
    /// <typeparam name="TMock">The mock target type.</typeparam>
    public interface IInvocationContext<TMock>
    {
        /// <summary>
        /// Tracks that the method represented by the <paramref name="expression"/>
        /// has been invoked.
        /// </summary>
        /// <param name="expression">The <see cref="Expression{TDelegate}"/> that 
        /// represents the method that has been invoked.</param>
        void Invoke(Expression<Action<TMock>> expression);

        /// <summary>
        /// Tracks that the method represented by the <paramref name="expression"/>
        /// has been invoked.
        /// </summary>
        /// <param name="expression">The <see cref="Expression{TDelegate}"/> that 
        /// represents the method that has been invoked.</param>
        /// <param name="refValues">A dictionary that will contain "out" or "ref" values
        /// if there is an appropriate callback.</param>
        void Invoke(Expression<Action<TMock>> expression, IDictionary<string, object>? refValues);

        /// <summary>
        /// Tracks that the method represented by the <paramref name="expression"/>
        /// has been invoked.
        /// </summary>
        /// <typeparam name="TResult">The return type of the method that has been invoked.</typeparam>
        /// <param name="expression">The <see cref="Expression{TDelegate}"/> that 
        /// represents the method that has been invoked.</param>
        [return: MaybeNull]
        TResult Invoke<TResult>(Expression<Func<TMock, TResult>> expression);

        /// <summary>
        /// Tracks that the method represented by the <paramref name="expression"/>
        /// has been invoked.
        /// </summary>
        /// <typeparam name="TResult">The return type of the method that has been invoked.</typeparam>
        /// <param name="expression">The <see cref="Expression{TDelegate}"/> that 
        /// represents the method that has been invoked.</param>
        /// <param name="refValues">A dictionary that will contain "out" or "ref" values
        /// if there is an appropriate callback.</param>
        /// <returns>An instance of <typeparamref name="TResult"/> or possibly null 
        /// if <typeparamref name="TResult"/> a reference type.</returns>
        [return: MaybeNull]
        TResult Invoke<TResult>(Expression<Func<TMock, TResult>> expression, IDictionary<string, object>? refValues);

        /// <summary>
        /// Tracks that the setter represented by the <paramref name="expression"/>
        /// has been invoked.
        /// </summary>
        /// <param name="expression">The <see cref="Expression{TDelegate}"/> that 
        /// represents the setter that has been invoked.</param>
        /// <param name="value">The value</param>
        void InvokeSetter<TResult>(Expression<Func<TMock, TResult>> expression, TResult value);
    }
}