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
using LightMock.Generator;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LightMock
{
    /// <summary>
    /// Advanced mock context methods
    /// </summary>
    /// <typeparam name="T">The target mock type</typeparam>
    public interface IAdvancedMockContext<T> : IMockContext<T>
    {
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
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertSet_When(Action<T> expression);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertSet_When(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="uidPart1">First part of uid that identifies <paramref name="expression"/></param>
        /// <param name="uidPart2">Second part of uid that identifies <paramref name="expression"/></param>
        /// <remarks>
        /// Usage restrictions:<br/>
        /// * All arguments MUST be available on compile time and be plain and simple;<br/>
        /// * Pattern matching using <see cref="The{TValue}"/> type is available;<br/>
        /// * Only one call per source code line allowed;<br/>
        /// * Do not place on same line <see cref="ArrangeSetter(Action{T}, string, int)"/>.
        /// * Works when code generation is enabled <see cref="DisableCodeGenerationAttribute"/>
        /// </remarks>
        void AssertSet(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0);
        /// <summary>
        /// Verifies that a property was set on the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <param name="uidPart1">First part of uid that identifies <paramref name="expression"/></param>
        /// <param name="uidPart2">Second part of uid that identifies <paramref name="expression"/></param>
        /// <remarks>
        /// Usage restrictions:<br/>
        /// * All arguments MUST be available on compile time and be plain and simple;<br/>
        /// * Pattern matching using <see cref="The{TValue}"/> type is available;<br/>
        /// * Only one call per source code line allowed;<br/>
        /// * Do not place on same line <see cref="ArrangeSetter(Action{T}, string, int)"/>.
        /// * Works when code generation is enabled <see cref="DisableCodeGenerationAttribute"/>
        /// </remarks>
        void AssertSet(Action<T> expression, Invoked times, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0);
        /// <summary>
        /// Verifies that a property was set with any value on the mock.
        /// </summary>
        /// <param name="propertySelector">Set a property that you want to check with any value</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertSet_WhenAny(Action<T> propertySelector);
        /// <summary>
        /// Verifies that a property was set with any value on the mock.
        /// </summary>
        /// <param name="propertySelector">Set a property that you want to check with any value</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertSet_WhenAny(Action<T> propertySelector, Invoked times);
        /// <summary>
        /// Verifies that an event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertAdd_When(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertRemove_When(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that an event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertAdd_When(Action<T> expression);
        /// <summary>
        /// Verifies that an event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertRemove_When(Action<T> expression);
        /// <summary>
        /// Verifies that any event was added to the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertAdd_WhenAny(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that any event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="times">The number of times a method is expected to be called.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertRemove_WhenAny(Action<T> expression, Invoked times);
        /// <summary>
        /// Verifies that any event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertAdd_WhenAny(Action<T> expression);
        /// <summary>
        /// Verifies that any event was removed from the mock.
        /// </summary>
        /// <param name="expression">Expression to verify.</param>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        void AssertRemove_WhenAny(Action<T> expression);

        /// <summary>
        /// Arranges a property setter.
        /// </summary>
        /// <param name="expression">The match expression that describes where this <see cref="IArrangement"/> will be applied.</param>
        /// <param name="uidPart1">First part of uid that identifies <paramref name="expression"/></param>
        /// <param name="uidPart2">Second part of uid that identifies <paramref name="expression"/></param>
        /// <returns>A new <see cref="IArrangement"/> used to apply property behavior.</returns>
        /// <remarks>
        /// Usage restrictions:<br/>
        /// * All arguments MUST be available on compile time and be plain and simple;<br/>
        /// * Pattern matching using <see cref="The{TValue}"/> type is available;<br/>
        /// * Only one call per source code line allowed;<br/>
        /// * Do not place on same line <see cref="AssertSet(Action{T}, string, int)"/>.
        /// * Works when code generation is enabled <see cref="DisableCodeGenerationAttribute"/>
        /// </remarks>
        IArrangement ArrangeSetter(Action<T> expression, [CallerFilePath] string uidPart1 = "", [CallerLineNumber] int uidPart2 = 0);
        /// <summary>
        /// Arranges a property setter when any value is set to it.
        /// </summary>
        /// <param name="expression">Expression used to select a property that will be arranged when any value is set</param>
        /// <returns>A new <see cref="IArrangement"/> used to apply property behavior.</returns>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        IArrangement ArrangeSetter_WhenAny(Action<T> expression);
        /// <summary>
        /// Arranges a property setter when specified value is set to it.
        /// </summary>
        /// <param name="expression">Expression used to select a property that will be arranged when specified value is set</param>
        /// <returns>A new <see cref="IArrangement"/> used to apply property behavior.</returns>
        /// <remarks>
        /// No AOT transformations, thus pattern matching using <see cref="The{TValue}"/> type is not working.
        /// </remarks>
        IArrangement ArrangeSetter_When(Action<T> expression);
    }

    internal interface IAdvancedMockContext
    {
        IEnumerable<IInvocationInfo> GetUnverifiedInvocations();
    }

}
