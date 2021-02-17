using System;

namespace LightMock.Generator
{
    [AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class DontOverrideAttribute : Attribute
    {
        /// <summary>
        /// Virtual functions in the specified class will not be overriden.
        /// </summary>
        /// <param name="type">Type of the class added to the exclusion list</param>
        public DontOverrideAttribute(Type type) { }
    }
}
