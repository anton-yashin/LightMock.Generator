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
using System.Threading;

namespace LightMock.Generator
{
    /// <summary>
    /// Caches result of type resolution in static variable
    /// </summary>
    /// <typeparam name="TTag">unique tag</typeparam>
    struct TypeCacher<TTag>
    {
        static Type? cache;

        /// <summary>
        /// Resolves type by <paramref name="typeResolver"/> if not yet resoved
        /// and activates it.
        /// </summary>
        /// <param name="typeResolver">Method to resolve type</param>
        /// <param name="args">Arguments to be passed in to constructor</param>
        /// <returns></returns>
        public object Activate(Func<Type> typeResolver, object[] args)
            => Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref cache,
                typeResolver), args: args);

        /// <summary>
        /// Resolves type by <paramref name="typeResolver"/> if not yet resoved
        /// and activates it.
        /// </summary>
        /// <param name="typeResolver">Method to resolve type</param>
        /// <returns></returns>
        public object Activate(Func<Type> typeResolver)
            => Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref cache,
                typeResolver));
    }
}
