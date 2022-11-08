using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LightMock
{
    /// <summary>
    /// For internal usage.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ArgumentHelper
    {
        /// <summary>
        /// For internal usage.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T Unpack<T>(IDictionary<string, object> arguments, string name, T @default)
            => arguments.TryGetValue(name, out var value) ? (T)value : @default;

        /// <summary>
        /// For internal usage.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Unpack<T>(IDictionary<string, object> arguments, string name)
            => Unpack<T>(arguments, name, default(T)!);
    }
}
