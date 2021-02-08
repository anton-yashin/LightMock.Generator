using System;
using System.ComponentModel;

namespace LightMock.Generator
{

    /// <summary>
    /// For internal usage
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDelegateProvider
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Delegate GetDelegate();
    }
}
