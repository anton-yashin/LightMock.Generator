using System;

namespace LightMock.Generator
{
    [AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DisableCodeGenerationAttribute : Attribute
    {
        public DisableCodeGenerationAttribute() { }
    }
}
