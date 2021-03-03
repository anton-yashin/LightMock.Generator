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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage.
    /// This class produces default values that will be returned by mock objects
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Default
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static void Get(Action action) => action();
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static T Get<T>(Func<T> func) => func();
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static Task Get(Func<Task> func)
            => func() ?? Task.CompletedTask;
#nullable disable
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static Task<T> Get<T>(Func<Task<T>> func) 
            => func() ?? Task.FromResult(default(T));
#nullable restore

        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static IEnumerable<T> Get<T>(Func<IEnumerable<T>> func)
            => func() ?? Enumerable.Empty<T>();

        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static Task<IEnumerable<T>> Get<T>(Func<Task<IEnumerable<T>>> func)
            => func() ?? Task.FromResult(Enumerable.Empty<T>());
    }
}
