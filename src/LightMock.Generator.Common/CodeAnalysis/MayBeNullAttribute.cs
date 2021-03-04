﻿namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field
        | AttributeTargets.Parameter
        | AttributeTargets.Property
        | AttributeTargets.ReturnValue,
        Inherited = false)]
    internal sealed class MaybeNullAttribute : Attribute
    { }
}