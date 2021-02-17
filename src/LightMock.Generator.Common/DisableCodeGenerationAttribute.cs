using System;

namespace LightMock.Generator
{
    /// <summary>
    /// Apply this attribute to your assembly to turn off this source code generator.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DisableCodeGenerationAttribute : Attribute
    {
        public DisableCodeGenerationAttribute() { }
    }
}
