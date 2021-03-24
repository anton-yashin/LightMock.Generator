using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage
    /// </summary>
    public sealed class OriginalNameAttribute : Attribute
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        public OriginalNameAttribute(int parametersCount, string originalNameFormat)
        {
            ParametersCount = parametersCount;
            OriginalNameFormat = originalNameFormat;
        }

        /// <summary>
        /// For internal usage
        /// </summary>
        public int ParametersCount { get; }

        /// <summary>
        /// For internal usage
        /// </summary>
        public string OriginalNameFormat { get; }
    }
}
