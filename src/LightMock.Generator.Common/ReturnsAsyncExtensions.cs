using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LightMock
{
    /// <summary>
    /// Extension methods to <see cref="IReturns{TResult}"/> interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ReturnsAsyncExtensions
    {
        /// <summary>
        /// Arranges the <paramref name="value"/> to return when method is called.
        /// </summary>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="this">The arrangement.</param>
        /// <param name="value">The value to return</param>
        /// <returns>Part of fluent interface.</returns>
        public static IReturnsResult<Task<TResult>> ReturnsAsync<TResult>(this IReturns<Task<TResult>> @this, TResult value)
        {
            return @this.Returns(Task.FromResult(value));
        }

        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="this">The arrangement.</param>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        public static IReturnsResult<Task<TResult>> ReturnsAsync<TResult>(this IReturns<Task<TResult>> @this, Func<TResult> getResultFunc)
        {
            return @this.Returns(Task.FromResult(getResultFunc()));
        }

        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="this">The arrangement.</param>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        public static IReturnsResult<Task<TResult>> ReturnsAsync<T1, TResult>(
            this IReturns<Task<TResult>> @this, Func<T1, TResult> getResultFunc)
        {
            return @this.Returns<T1>((a1) => Task.FromResult(getResultFunc(a1)));
        }

        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="this">The arrangement.</param>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        public static IReturnsResult<Task<TResult>> ReturnsAsync<T1, T2, TResult>(
            this IReturns<Task<TResult>> @this, Func<T1, T2, TResult> getResultFunc)
        {
            return @this.Returns<T1, T2>((a1, a2) => Task.FromResult(getResultFunc(a1, a2)));
        }

        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <typeparam name="T4">Type of fourth parameter of mocked method.</typeparam>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="this">The arrangement.</param>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        public static IReturnsResult<Task<TResult>> ReturnsAsync<T1, T2, T3, TResult>(
            this IReturns<Task<TResult>> @this, Func<T1, T2, T3, TResult> getResultFunc)
        {
            return @this.Returns<T1, T2, T3>((a1, a2, a3) => Task.FromResult(getResultFunc(a1, a2, a3)));
        }

        /// <summary>
        /// Arranges a factory <paramref name="getResultFunc"/> that will be called to generate value to return when method is called.
        /// </summary>
        /// <typeparam name="T1">Type of first parameter of mocked method.</typeparam>
        /// <typeparam name="T2">Type of second parameter of mocked method.</typeparam>
        /// <typeparam name="T3">Type of third parameter of mocked method.</typeparam>
        /// <typeparam name="T4">Type of fourth parameter of mocked method.</typeparam>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="this">The arrangement.</param>
        /// <param name="getResultFunc">The factory of <typeparamref name="TResult"/></param>
        /// <returns>Part of fluent interface.</returns>
        public static IReturnsResult<Task<TResult>> ReturnsAsync<T1, T2, T3, T4, TResult>(
            this IReturns<Task<TResult>> @this, Func<T1, T2, T3, T4, TResult> getResultFunc)
        {
            return @this.Returns<T1, T2, T3, T4>((a1, a2, a3, a4) => Task.FromResult(getResultFunc(a1, a2, a3, a4)));
        }
    }
}
