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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LightMock.Generator
{
    sealed class ImmutableArraySegment<T> : IReadOnlyList<T>
    {
        private readonly ImmutableArray<T> underlying;
        private readonly int start;
        private readonly int length;

        public ImmutableArraySegment(ImmutableArray<T> underlying, int start, int length)
        {
            if (start < 0 || underlying.Length < start)
                throw new ArgumentOutOfRangeException(nameof(start));
            if ((underlying.Length - start) < length)
                throw new ArgumentOutOfRangeException(nameof(length));
            this.underlying = underlying;
            this.start = start;
            this.length = length;
        }

        public T this[int index]
        {
            get
            {
                if (index >= length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return underlying[index + start];
            }
        }

        public int Count => length;

        public IEnumerator<T> GetEnumerator()
        {
            var last = start + length;
            for (int i = start; i < last; i++)
                yield return underlying[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
