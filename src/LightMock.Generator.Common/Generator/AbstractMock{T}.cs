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
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LightMock.Generator
{
    public abstract class AbstractMock<T> : IProtectedContext<T>, IMock<T>, IMockContext<T>
        where T : class
    {
        T? instance;
        readonly object[] prms;
        readonly object protectedContext;
        readonly IMockContextInternal propertiesContext;

        public AbstractMock()
        {
            prms = Array.Empty<object>();

            PublicContext = new MockContext<T>();
            protectedContext = CreateProtectedContext();
            propertiesContext = CreatePropertiesContext();
        }

        public AbstractMock(params object[] prms) : this()
        {
            this.prms = prms;
        }

        public T Object => LazyInitializer.EnsureInitialized(ref instance!, CreateMockInstance);

        object IProtectedContext<T>.ProtectedContext => protectedContext;
        protected IMockContext<T> PublicContext { get; }

        static Type? mockInstanceType;
        static Type? protectedContextType;
        static Type? propertiesType;
        static Type? assertType;

        object[] GetArgs()
        {
            const int offset = 3;
            var args = new object[prms.Length + offset];
            args[0] = PublicContext;
            args[1] = propertiesContext;
            args[2] = protectedContext;
            for (int i = 0; i < prms.Length; i++)
                args[i + offset] = prms[i];
            return args;
        }

        object[] GetAssertArgs(Invoked invoked)
        {
            var args = new object[prms.Length + 2];
            args[0] = propertiesContext;
            args[1] = invoked;
            for (int i = 0; i < prms.Length; i++)
                args[i + 2] = prms[i];
            return args;
        }

        T CreateMockInstance()
        {
            var type = LazyInitializer.EnsureInitialized(ref mockInstanceType!, GetInstanceType);
            if (type.IsDelegate())
                return GetDelegate(type);
            var result = Activator.CreateInstance(type, args: GetArgs())
                ?? throw new InvalidOperationException("can't create context for: " + typeof(T).FullName);
            return (T)result;
        }

        object CreateProtectedContext()
        {
            return Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref protectedContextType,
                GetProtectedContextType))
                ?? throw new InvalidOperationException("can't create protected context for: " + typeof(T).FullName);
        }

        T CreateAssertInstance(Invoked invoked)
        {
            var result = Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref assertType,
                GetAssertType), args: GetAssertArgs(invoked))
                ?? throw new InvalidOperationException("can't create assert for: " + typeof(T).FullName);
            return (T)result;
        }

        IMockContextInternal CreatePropertiesContext()
        {
            return (IMockContextInternal)Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref propertiesType,
                GetPropertiesContextType))
                ?? throw new InvalidOperationException("can't create property context for: " + typeof(T).FullName);
        }

        Type GetInstanceType()
            => GetInstanceType(ContextResolverDefaults.Instance);
        Type GetProtectedContextType()
            => GetProtectedContextType(ContextResolverDefaults.Instance);
        Type GetPropertiesContextType()
            => GetPropertiesContextType(ContextResolverDefaults.Instance);
        Type GetAssertType()
            => GetAssertType(ContextResolverDefaults.Instance);
        T GetDelegate(Type type)
            => GetDelegate(type, ContextResolverDefaults.Instance);
        LambdaExpression ExchangeForExpression(string token)
            => ExchangeForExpression(token, ContextResolverDefaults.Instance);

        protected abstract Type GetInstanceType(IContextResolverDefaults defaults);
        protected abstract Type GetProtectedContextType(IContextResolverDefaults defaults);
        protected abstract Type GetPropertiesContextType(IContextResolverDefaults defaults);
        protected abstract Type GetAssertType(IContextResolverDefaults defaults);
        protected abstract T GetDelegate(Type type, IContextResolverDefaults defaults);
        protected abstract LambdaExpression ExchangeForExpression(string token, IContextResolverDefaults defaults);

        public void AssertGet<TProperty>(Func<T, TProperty> expression)
            => AssertGet(expression, Invoked.AtLeast(1));

        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
            => expression(CreateAssertInstance(times));

        public void AssertSet_Simple(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertSet_Simple(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertAdd(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertAdd(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertRemove(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertRemove(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        void AssertUsingAssertInstance(Action<T> expression, Invoked times)
            => expression(CreateAssertInstance(times));

        const string KUidExceptionMessage = "you must provide part of unique identifier";

        public IArrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            return propertiesContext.ArrangeAction(ExchangeForExpression(uidPart2 + uidPart1));
        }

        public void AssertSet(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => AssertSet(expression, Invoked.AtLeast(1), uidPart1, uidPart2);

        public void AssertSet(Action<T> expression, Invoked times, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            propertiesContext.AssertInternal(ExchangeForExpression(uidPart2 + uidPart1), times);
        }

        #region IMockContext<T> implementation

        public IArrangement Arrange(Expression<Action<T>> matchExpression)
            => PublicContext.Arrange(matchExpression);

        public IArrangement<TResult> Arrange<TResult>(Expression<Func<T, TResult>> matchExpression)
            => PublicContext.Arrange(matchExpression);

        public IArrangement ArrangeProperty<TResult>(Expression<Func<T, TResult>> matchExpression)
            => PublicContext.ArrangeProperty(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression)
            => PublicContext.Assert(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression, Invoked invoked)
            => PublicContext.Assert(matchExpression, invoked);

        #endregion
    }
}
