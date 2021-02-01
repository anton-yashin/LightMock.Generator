using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[Obsolete]
public sealed class GenerateMockAttribute : Attribute
{
    public GenerateMockAttribute() { }
}
