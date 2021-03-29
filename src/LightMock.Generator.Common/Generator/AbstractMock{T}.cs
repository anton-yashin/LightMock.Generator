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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace LightMock.Generator
{
    /// <summary>
    /// Base class for generated Mock&lt;T&gt; class.
    /// </summary>
    /// <typeparam name="T">The type for which mock is generated.</typeparam> 
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class AbstractMock<T> : IProtectedContext<T>, IMock<T>, IMockContext<T>
        where T : class
    {
        T? instance;
        readonly object[] prms;
        readonly TypeResolver typeResolver;
        readonly IAdvancedMockContext protectedContext;
        readonly IMockContextInternal propertiesContext;
        readonly AdvancedMockContext<T> publicContext;

        /// <summary>
        /// Initializes a contexts which you can use to arrange a behaviour.
        /// </summary>
        /// <param name="prms">Paramters that must be used to initalize base class.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected AbstractMock(object[] prms)
        {
            if (ContextResolverTable.TryGetValue(resolverType, out var t) == false)
                throw new MockNotGeneratedException(contextType);

            this.prms = prms;
            typeResolver = (TypeResolver)Activator.CreateInstance(t, contextType);
            publicContext = new AdvancedMockContext<T>(prms, typeResolver, ExchangeForExpression);
            protectedContext = typeResolver.ActivateProtectedContext<T>(GetProtectedContextArgs());
            propertiesContext = typeResolver.GetPropertiesContext<T>();
        }

        /// <summary>
        /// Instance of generated mock object.
        /// </summary>
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

        object[] GetProtectedContextArgs()
        {
            var args = new object[3];
            args[0] = prms;
            args[1] = typeResolver;
            args[2] = new Func<string, LambdaExpression>(ExchangeForExpression);
            return args;
        }

        T CreateMockInstance()
        {
            if (isDelegate)
                return (T)typeResolver.GetDelegate(publicContext);
            return typeResolver.ActivateInstance<T>(GetMockInstanceArgs());
        }

        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected abstract LambdaExpression ExchangeForExpression(string token);
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected abstract IReadOnlyDictionary<Type, Type> ContextResolverTable { get; }

        #region IAdvancedMockContext<T> implementation

        ///<inheritdoc/>
        public void AssertGet<TProperty>(Func<T, TProperty> expression)
            => publicContext.AssertGet(expression);

        ///<inheritdoc/>
        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
            => publicContext.AssertGet(expression, times);

        ///<inheritdoc/>
        public void AssertSet_When(Action<T> expression)
            => publicContext.AssertSet_When(expression);

        ///<inheritdoc/>
        public void AssertSet_When(Action<T> expression, Invoked times)
            => publicContext.AssertSet_When(expression, times);

        ///<inheritdoc/>
        public void AssertAdd(Action<T> expression)
            => publicContext.AssertAdd(expression);

        ///<inheritdoc/>
        public void AssertAdd(Action<T> expression, Invoked times)
            => publicContext.AssertAdd(expression, times);

        ///<inheritdoc/>
        public void AssertRemove(Action<T> expression)
            => publicContext.AssertRemove(expression);

        ///<inheritdoc/>
        public void AssertRemove(Action<T> expression, Invoked times)
            => publicContext.AssertRemove(expression, times);

        ///<inheritdoc/>
        public void AssertAdd_WhenAny(Action<T> expression)
            => publicContext.AssertAdd_WhenAny(expression);

        ///<inheritdoc/>
        public void AssertAdd_WhenAny(Action<T> expression, Invoked times)
            => publicContext.AssertAdd_WhenAny(expression, times);

        ///<inheritdoc/>
        public void AssertRemove_WhenAny(Action<T> expression)
            => publicContext.AssertRemove_WhenAny(expression);

        ///<inheritdoc/>
        public void AssertRemove_WhenAny(Action<T> expression, Invoked times)
            => publicContext.AssertRemove_WhenAny(expression, times);

        ///<inheritdoc/>
        public IArrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => publicContext.ArrangeSetter(expression, uidPart1, uidPart2);

        ///<inheritdoc/>
        public IArrangement ArrangeSetter_WhenAny(Action<T> expression)
                => publicContext.ArrangeSetter_WhenAny(expression);

        ///<inheritdoc/>
        public IArrangement ArrangeSetter_When(Action<T> expression)
            => publicContext.ArrangeSetter_When(expression);

        ///<inheritdoc/>
        public void AssertSet(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => publicContext.AssertSet(expression, uidPart1, uidPart2);

        ///<inheritdoc/>
        public void AssertSet(Action<T> expression, Invoked times, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => publicContext.AssertSet(expression, times, uidPart1, uidPart2);

        ///<inheritdoc/>
        public void AssertSet_WhenAny(Action<T> expression)
            => publicContext.AssertSet_WhenAny(expression);

        ///<inheritdoc/>
        public void AssertSet_WhenAny(Action<T> expression, Invoked times)
            => publicContext.AssertSet_WhenAny(expression, times);

        ///<inheritdoc/>
        public void AssertNoOtherCalls()
        {
            var unverifiedInvocations = publicContext.GetUnverifiedInvocations()
                .Concat(protectedContext.GetUnverifiedInvocations())
                .Concat(propertiesContext.GetUnverifiedInvocations())
                .Where(i => i.IsMethod).ToArray();
            if (unverifiedInvocations.Length > 0)
            {
                var messageBuilder = new StringBuilder().AppendLine("Detected unverified invocations: ");
                foreach (var i in unverifiedInvocations)
                {
                    i.AppendInvocationInfo(messageBuilder);
                    messageBuilder.AppendLine();
                }
                throw new MockException(messageBuilder.ToString());
            }
        }

        #endregion

        #region IMockContext<T> implementation

        ///<inheritdoc/>
        public IArrangement Arrange(Expression<Action<T>> matchExpression)
            => publicContext.Arrange(matchExpression);

        ///<inheritdoc/>
        public IArrangement<TResult> Arrange<TResult>(Expression<Func<T, TResult>> matchExpression)
            => publicContext.Arrange(matchExpression);

        ///<inheritdoc/>
        public IArrangement ArrangeProperty<TResult>(Expression<Func<T, TResult>> matchExpression)
            => publicContext.ArrangeProperty(matchExpression);

        ///<inheritdoc/>
        public void Assert(Expression<Action<T>> matchExpression)
            => publicContext.Assert(matchExpression);

        ///<inheritdoc/>
        public void Assert(Expression<Action<T>> matchExpression, Invoked invoked)
            => publicContext.Assert(matchExpression, invoked);

        #endregion

    }
}
