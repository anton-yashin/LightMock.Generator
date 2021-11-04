using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ILambdaToken
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// For internal usage
        /// </summary>
        public LambdaExpression Value { get; }
    }
}
