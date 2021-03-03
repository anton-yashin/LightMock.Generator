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

namespace LightMock
{
    sealed class LockedCollection<T> : ICollection<T>, ILockedCollection<T>
    {
        private readonly List<T> collection;

        public LockedCollection()
            => this.collection = new List<T>();

        public int Count => InvokeLocked(c => c.Count);

        public bool IsReadOnly => false;

        public void Add(T item) => InvokeLocked(c => c.Add(item));

        public void Clear() => InvokeLocked(c => c.Clear());

        public bool Contains(T item) => InvokeLocked(c => c.Contains(item));

        public void CopyTo(T[] array, int arrayIndex)
            => InvokeLocked(c => c.CopyTo(array, arrayIndex));


        public IEnumerator<T> GetEnumerator()
            => ((IEnumerable<T>)ToArray()).GetEnumerator();

        public bool Remove(T item) => InvokeLocked(c => c.Remove(item));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T[] ToArray()
        {
            lock (collection)
                return collection.ToArray();
        }

        public void InvokeLocked(Action<ICollection<T>> action)
        {
            lock (collection)
                action(collection);
        }

        public TResult InvokeLocked<TResult>(Func<ICollection<T>, TResult> func)
        {
            lock (collection)
                return func(collection);
        }
    }
}
