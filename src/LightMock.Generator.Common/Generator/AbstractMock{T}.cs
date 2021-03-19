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
        readonly MockContext<T> publicContext;
        readonly object protectedContext;
        readonly IMockContextInternal propertiesContext;

        /// <summary>
        /// Initializes a contexts which you can use to arrange a behaviour.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected AbstractMock()
        {
            if (ContextResolverTable.TryGetValue(resolverType, out var t) == false)
                throw new MockNotGeneratedException(contextType);

            prms = Array.Empty<object>();
            typeResolver = (TypeResolver)Activator.CreateInstance(t, contextType);
            publicContext = new MockContext<T>();
            protectedContext = typeResolver.ActivateProtectedContext<T>();
            propertiesContext = typeResolver.ActivatePropertiesContext<T>();
        }

        /// <summary>
        /// Initializes a contexts which you can use to arrange a behaviour.
        /// </summary>
        /// <param name="prms">Paramters that must be used to initalize base class.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected AbstractMock(params object[] prms) : this()
        {
            this.prms = prms;
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

        T CreateAssertWhenInstance(Invoked invoked)
            => typeResolver.ActivateAssertWhenInstance<T>(GetAssertArgs(invoked));

        T CreateAssertWhenAnyInstance(Invoked invoked)
            => typeResolver.ActivateAssertWhenAnyInstance<T>(GetAssertArgs(invoked));

        T CreateArrangeWhenAnyInstance(ILambdaRequest request)
            => typeResolver.ActivateArrangeWhenAnyInstance<T>(GetArrangeArgs(request));

        T CreateArrangeWhenInstance(ILambdaRequest request)
            => typeResolver.ActivateArrangeWhenInstance<T>(GetArrangeArgs(request));

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

        ///<inheritdoc/>
        public void AssertNoOtherCalls()
        {
            var publicInvocations = publicContext.GetUnverifiedCalls();
            var propertyInvocations = propertiesContext.GetUnverifiedCalls();
            var protectedInvocations = protectedContext is IMockContextInternal mci
                ? mci.GetUnverifiedCalls() : Array.Empty<IInvocationInfo>();
            if (publicInvocations.Any() || propertyInvocations.Any() || protectedInvocations.Any())
            {
                var message = new StringBuilder("Detected unverified invocations: ");
                message.AppendLine();
                foreach (var i in publicInvocations)
                    message.Append(i.AsString()).AppendLine();

                foreach (var i in propertyInvocations)
                    message.Append(i.AsString()).AppendLine();

                foreach (var i in protectedInvocations)
                    message.Append(i.AsString()).AppendLine();

                throw new MockException(message.ToString());
            }
        }

        ///<inheritdoc/>
        public void AssertGet<TProperty>(Func<T, TProperty> expression)
            => AssertGet(expression, Invoked.AtLeast(1));

        ///<inheritdoc/>
        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
            => expression(CreateAssertWhenInstance(times));

        ///<inheritdoc/>
        public void AssertSet_When(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        ///<inheritdoc/>
        public void AssertSet_When(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        ///<inheritdoc/>
        public void AssertAdd(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        ///<inheritdoc/>
        public void AssertAdd(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        ///<inheritdoc/>
        public void AssertRemove(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.AtLeast(1));

        ///<inheritdoc/>
        public void AssertRemove(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        void AssertUsingAssertInstance(Action<T> expression, Invoked times)
            => expression(CreateAssertWhenInstance(times));

        const string KUidExceptionMessage = "you must provide part of unique identifier";

        ///<inheritdoc/>
        public IArrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            return propertiesContext.ArrangeAction(ExchangeForExpression(uidPart2 + uidPart1));
        }

        ///<inheritdoc/>
        public IArrangement ArrangeSetter_WhenAny(Action<T> expression)
            => ArrangeSetter_NoAot(expression, CreateArrangeWhenAnyInstance);

        ///<inheritdoc/>
        public IArrangement ArrangeSetter_When(Action<T> expression) 
            => ArrangeSetter_NoAot(expression, CreateArrangeWhenInstance);

        IArrangement ArrangeSetter_NoAot(Action<T> expression, Func<ILambdaRequest, T> instanceFactory)
        {
            var request = new LambdaRequest();
            expression(instanceFactory(request));
            var result = request.Result ?? throw new InvalidOperationException("A property assignment is required.");
            return propertiesContext.ArrangeAction(result);
        }

        ///<inheritdoc/>
        public void AssertSet(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
            => AssertSet(expression, Invoked.AtLeast(1), uidPart1, uidPart2);

        ///<inheritdoc/>
        public void AssertSet(Action<T> expression, Invoked times, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException(KUidExceptionMessage, nameof(uidPart1));
            propertiesContext.AssertInternal(ExchangeForExpression(uidPart2 + uidPart1), times);
        }

        ///<inheritdoc/>
        public void AssertSet_WhenAny(Action<T> expression)
            => AssertSet_WhenAny(expression, Invoked.AtLeast(1));

        ///<inheritdoc/>
        public void AssertSet_WhenAny(Action<T> expression, Invoked times)
            => expression(CreateAssertWhenAnyInstance(times));

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
