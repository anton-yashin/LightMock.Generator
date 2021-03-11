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
        readonly TypeResolver typeResolver;
        readonly IMockContext<T> publicContext;
        readonly object protectedContext;
        readonly IMockContextInternal propertiesContext;

        public AbstractMock()
        {
            if (ContextResolverTable.TryGetValue(resolverType, out var t) == false)
                throw new MockNotGeneratedException(contextType);

            prms = Array.Empty<object>();
            typeResolver = (TypeResolver)Activator.CreateInstance(t, contextType);
            publicContext = new MockContext<T>();
            protectedContext = typeResolver.ActivateProtectedContext<T>();
            propertiesContext = typeResolver.ActivatePropertiesContext<T>();
        }

        public AbstractMock(params object[] prms) : this()
        {
            this.prms = prms;
        }

        public T Object => LazyInitializer.EnsureInitialized(ref instance!, CreateMockInstance);

        object IProtectedContext<T>.ProtectedContext => protectedContext;

        static readonly Type contextType = typeof(T);
        static readonly Type resolverType = contextType.IsGenericType ? contextType.GetGenericTypeDefinition() : contextType;
        static readonly bool isDelegate = contextType.IsDelegate();

        object[] GetMockInstanceArgs()
        {
            const int offset = 3;
            var args = new object[prms.Length + offset];
            args[0] = publicContext;
            args[1] = propertiesContext;
            args[2] = protectedContext;
            Array.Copy(prms, 0, args, offset, prms.Length);
            return args;
        }

        object[] GetAssertArgs(Invoked invoked)
        {
            const int offset = 2;
            var args = new object[prms.Length + offset];
            args[0] = propertiesContext;
            args[1] = invoked;
            Array.Copy(prms, 0, args, offset, prms.Length);
            return args;
        }

        object[] GetArrangeArgs(ILambdaRequest request)
        {
            const int offset = 1;
            var args = new object[prms.Length + offset];
            args[0] = request;
            Array.Copy(prms, 0, args, offset, prms.Length);
            return args;
        }

        T CreateMockInstance()
        {
            if (isDelegate)
                return (T)typeResolver.GetDelegate(publicContext);
            return typeResolver.ActivateInstance<T>(GetMockInstanceArgs());
        }

        T CreateAssertInstance(Invoked invoked)
            => typeResolver.ActivateAssertInstance<T>(GetAssertArgs(invoked));

        T CreateAssertIsAnyInstance(Invoked invoked)
            => typeResolver.ActivateAssertIsAnyInstance<T>(GetAssertArgs(invoked));

        T CreateArrangeOnAnyInstance(ILambdaRequest request)
            => typeResolver.ActivateArrangeOnAnyInstance<T>(GetArrangeArgs(request));

        T CreateArrangeOnInstance(ILambdaRequest request)
            => typeResolver.ActivateArrangeOnInstance<T>(GetArrangeArgs(request));

        LambdaExpression ExchangeForExpression(string token)
            => ExchangeForExpression(token, ContextResolverDefaults.Instance);

        protected abstract LambdaExpression ExchangeForExpression(string token, IContextResolverDefaults defaults);
        protected abstract IReadOnlyDictionary<Type, Type> ContextResolverTable { get; }

        public void AssertGet<TProperty>(Func<T, TProperty> expression)
            => AssertGet(expression, Invoked.AtLeast(1));

        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
            => expression(CreateAssertInstance(times));

        public void AssertSet_NoAot(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        public void AssertSet_NoAot(Action<T> expression, Invoked times)
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

        public IArrangement ArrangeSetter_OnAny(Action<T> expression)
            => ArrangeSetter_NoAot(expression, CreateArrangeOnAnyInstance);

        public IArrangement ArrangeSetter_On(Action<T> expression) 
            => ArrangeSetter_NoAot(expression, CreateArrangeOnInstance);

        IArrangement ArrangeSetter_NoAot(Action<T> expression, Func<ILambdaRequest, T> instanceFactory)
        {
            var request = new LambdaRequest();
            expression(instanceFactory(request));
            var result = request.Result ?? throw new InvalidOperationException("A property assignment is required.");
            return propertiesContext.ArrangeAction(result);
        }

        public void AssertSet(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => AssertSet(expression, Invoked.AtLeast(1), uidPart1, uidPart2);

        public void AssertSet(Action<T> expression, Invoked times, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            propertiesContext.AssertInternal(ExchangeForExpression(uidPart2 + uidPart1), times);
        }

        public void AssertSet_IsAny(Action<T> expression)
            => AssertSet_IsAny(expression, Invoked.AtLeast(1));

        public void AssertSet_IsAny(Action<T> expression, Invoked times)
            => expression(CreateAssertIsAnyInstance(times));

        #region IMockContext<T> implementation

        public IArrangement Arrange(Expression<Action<T>> matchExpression)
            => publicContext.Arrange(matchExpression);

        public IArrangement<TResult> Arrange<TResult>(Expression<Func<T, TResult>> matchExpression)
            => publicContext.Arrange(matchExpression);

        public IArrangement ArrangeProperty<TResult>(Expression<Func<T, TResult>> matchExpression)
            => publicContext.ArrangeProperty(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression)
            => publicContext.Assert(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression, Invoked invoked)
            => publicContext.Assert(matchExpression, invoked);

        #endregion

    }
}
