using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class GenerateMockAttribute : Attribute
{
    public GenerateMockAttribute() { }
}
