/******************************************************************************
    MIT License

    Copyright (c) 2022 Anton Yashin

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
using System.ComponentModel;

namespace LightMock
{
    public partial class The<TValue>
    {
        /// <summary>
        /// Specifies an <see langword="ref"/> argument match.
        /// </summary>
        public static class Reference
        {
            /// <summary>
            /// Specifies that the <see langword="ref"/> argument value can be any value of <typeparamref name="TValue"/>.
            /// </summary>
            /// <remarks>You can use this with <see langword="out"/> argument, but remember that
            /// you can't capture incoming value of <see langword="out"/> argument.</remarks>
            public static TheReference<TValue> IsAny => new TheReference<TValue>();

            /// <summary>
            /// Specifies that the <see langword="ref"/> argument value must match the given <paramref name="predicate"/>.
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns>New <see cref="TheReference{TValue}"/></returns>
            /// <remarks>This pattern matching does not work with <see langword="out"/> argments,
            /// because you can't capture incomming value of <see langword="out"/> argments.</remarks>
            public static TheReference<TValue> Is(Func<TValue, bool> predicate) => new TheReference<TValue>();

        }
    }
    /// <summary>
    /// Contains a marker for a <see langword="ref"/> argument
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class TheReference<TValue>
    {
        /// <summary>
        /// The marker for a <see langword="ref"/> argument
        /// </summary>
#nullable disable
        public TValue Value;
#nullable enable
    }
}
