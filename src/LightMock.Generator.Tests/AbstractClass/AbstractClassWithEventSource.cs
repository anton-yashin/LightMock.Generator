﻿using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public delegate void EventHandler<T>(object source, T data);
    
    public abstract class AAbstractClassWithEventSource
    {
#pragma warning disable CS0067 // never used
        public virtual event EventHandler<int>? OnVirtualEvent;
        protected virtual event EventHandler<int>? OnProtectedVirtualEvent;
        protected internal virtual event EventHandler<int>? OnProtectedInternalVirtualEvent;
#pragma warning restore CS0067 // never used
        public abstract event EventHandler<int>? OnAbstractEvent;
        protected abstract event EventHandler<int>? OnProtectedAbstractEvent;
        protected internal abstract event EventHandler<int>? OnProtectedInternalAbstractEvent;
    }
}
