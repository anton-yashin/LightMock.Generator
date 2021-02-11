using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage.
    /// This class produces default values that will be returned by mock objects
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Default
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static void Get(Action action) => action();
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static T Get<T>(Func<T> func) => func();
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static Task Get(Func<Task> func)
            => func() ?? Task.CompletedTask;
#nullable disable
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static Task<T> Get<T>(Func<Task<T>> func) 
            => func() ?? Task.FromResult(default(T));
#nullable restore

        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static IEnumerable<T> Get<T>(Func<IEnumerable<T>> func)
            => func() ?? Enumerable.Empty<T>();

        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
        public static Task<IEnumerable<T>> Get<T>(Func<Task<IEnumerable<T>>> func)
            => func() ?? Task.FromResult(Enumerable.Empty<T>());
    }
}
