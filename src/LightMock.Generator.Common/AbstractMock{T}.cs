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
        readonly object propertiesContext;

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
        protected MockContext<T> PublicContext { get; }

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

        object CreatePropertiesContext()
        {
            return Activator.CreateInstance(LazyInitializer.EnsureInitialized(ref propertiesType,
                GetPropertiesContextType))
                ?? throw new InvalidOperationException("can't create property context for: " + typeof(T).FullName);
        }


        protected abstract Type GetInstanceType();
        protected abstract Type GetProtectedContextType();
        protected abstract Type GetPropertiesContextType();
        protected abstract Type GetAssertType();
        protected abstract T GetDelegate(Type type);
        protected abstract Expression<Action<T>> ExchangeForExpression(string token);

        public void AssertGet<TProperty>(Func<T, TProperty> expression)
            => AssertGet(expression, Invoked.Once);

        public void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times)
            => expression(CreateAssertInstance(times));

        public void AssertSet(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.Once);

        public void AssertSet(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertAdd(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.Once);

        public void AssertAdd(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        public void AssertRemove(Action<T> expression)
            => AssertUsingAssertInstance(expression, Invoked.Once);

        public void AssertRemove(Action<T> expression, Invoked times)
            => AssertUsingAssertInstance(expression, times);

        void AssertUsingAssertInstance(Action<T> expression, Invoked times)
            => expression(CreateAssertInstance(times));

        public Arrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0)
        {
            if (string.IsNullOrWhiteSpace(uidPart1))
                throw new ArgumentException("you must provide part of unique identifier", nameof(uidPart1));
            return Arrange(ExchangeForExpression(uidPart1 + uidPart2.ToString()));
        }

        #region IMockContext<T> implementation

        public Arrangement Arrange(Expression<Action<T>> matchExpression)
            => PublicContext.Arrange(matchExpression);

        public Arrangement<TResult> Arrange<TResult>(Expression<Func<T, TResult>> matchExpression)
            => PublicContext.Arrange(matchExpression);

        public PropertyArrangement<TResult> ArrangeProperty<TResult>(Expression<Func<T, TResult>> matchExpression)
            => PublicContext.ArrangeProperty(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression)
            => PublicContext.Assert(matchExpression);

        public void Assert(Expression<Action<T>> matchExpression, Invoked invoked)
            => PublicContext.Assert(matchExpression, invoked);

        #endregion
    }
}
