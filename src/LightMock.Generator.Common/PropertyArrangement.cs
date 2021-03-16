using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace LightMock
{
    /// <summary>
    /// A class that represents an arrangement of a mocked property that 
    /// returns a value of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the mocked property.</typeparam>
    sealed class PropertyArrangement<TResult> : Arrangement,
        IPropertyArrangementInvocation<TResult>,
        IArrangementInvocation<TResult>
    {
        [AllowNull]
        private TResult result;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyArrangement{TResult}"/> class.
        /// </summary>
        /// <param name="expression">The <see cref="LambdaExpression"/> that specifies
        /// where to apply this <see cref="Arrangement"/>.</param>
        public PropertyArrangement(LambdaExpression expression)
            : base(expression)
        {
            result = default;
        }

        [return: MaybeNull]
        TResult IArrangementInvocation<TResult>.Invoke(IInvocationInfo invocation) => result;

        void IPropertyArrangementInvocation<TResult>.Invoke(TResult value) => result = value;

    }
}