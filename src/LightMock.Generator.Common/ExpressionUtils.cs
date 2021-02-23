using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionUtils
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Expression<Action<T>> Get<T>(Expression<Action<T>> expression) => expression;
    }
}
