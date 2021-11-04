using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    /// <summary>
    /// For internal usage
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TypeKeyAttribute : Attribute
    {
        /// <summary>
        /// For internal usage
        /// </summary>
        public TypeKeyAttribute(Type key) => Key = key;

        /// <summary>
        /// For internal usage
        /// </summary>
        public Type Key { get; }
    }
}
