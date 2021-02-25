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
using System.Runtime.CompilerServices;

namespace LightMock.Generator
{
    public interface IMock<T> : IMockContext<T>
        where T : class
    {
        /// <summary>
        /// Exposes the mocked object instance.
        /// </summary>
        T Object { get; }

        /// <summary>
        /// Verifies that a property was read on the mock.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property to verify</typeparam>
        /// <param name="expression">Expression to verify.</param>
        void AssertGet<TProperty>(Func<T, TProperty> expression);
        /// <summary>
        /// Verifies that a property was read on the mock.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property to verify</typeparam>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        void AssertSet(Action<T> expression);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertSet(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertAdd(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertRemove(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        void AssertAdd(Action<T> expression);
        /// <summary>
        /// Verifies that an event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertRemove(Action<T> expression);

        Arrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0);
    }
}
