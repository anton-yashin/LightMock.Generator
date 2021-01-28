using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateMockAttribute : Attribute
{
    public GenerateMockAttribute() { }
}
