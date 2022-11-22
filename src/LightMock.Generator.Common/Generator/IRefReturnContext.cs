using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IRefReturnContext<T>
        where T : class
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        object RefReturnContext { get; }
    }
}
