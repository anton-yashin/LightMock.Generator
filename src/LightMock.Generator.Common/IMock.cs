using System;

namespace LightMock.Generator
{
    public interface IMock<T> : IMockContext<T>
        where T : class
    {
        /// <summary>
        /// Exposes the mocked object instance.
        /// </summary>
        T Object { get; }

        /// <summary>
        /// Verifies that a property was read on the mock.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property to verify</typeparam>
        /// <param name="expression">Expression to verify.</param>
        void AssertGet<TProperty>(Func<T, TProperty> expression);
        /// <summary>
        /// Verifies that a property was read on the mock.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property to verify</typeparam>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertGet<TProperty>(Func<T, TProperty> expression, Invoked times);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        void AssertSet(Action<T> expression);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertSet(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertAdd(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertRemove(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        void AssertAdd(Action<T> expression);
        /// <summary>
        /// Verifies that an event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        void AssertRemove(Action<T> expression);
    }
}
