using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class MockGeneratedAttribute : Attribute
{
    public MockGeneratedAttribute() { }
}
