﻿using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public delegate void EventHandlerGenericClass<T>(object source, T data);

    public abstract class AGenericAbstractClassWithGenericEvent<T>
    {
        public abstract event EventHandlerGenericClass<T> OnAbstractEvent;
        protected abstract event EventHandlerGenericClass<T> OnProtectedAbstractEvent;
#pragma warning disable CS0067 // never used
        public virtual event EventHandlerGenericClass<T>? OnVirtualEvent;
        protected virtual event EventHandlerGenericClass<T>? OnProtectedVirtualEvent;
#pragma warning restore CS0067 // never used
    }
}
