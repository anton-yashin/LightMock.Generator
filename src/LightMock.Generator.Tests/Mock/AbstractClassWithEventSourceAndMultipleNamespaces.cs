﻿using System;

namespace LightMock.Generator.Tests.Mock.EventNamespace2
{
    using EventNamespace1;

    public abstract class AAbstractClassWithEventSourceAndMultipleNamespaces
    {
#pragma warning disable CS0067 // never used
        public virtual event EventHandler<int>? OnVirtualEvent;
        protected virtual event EventHandler<int>? OnProtectedVirtualEvent;
#pragma warning restore CS0067 // never used
        public abstract event EventHandler<int>? OnAbstractEvent;
        protected abstract event EventHandler<int>? OnProtectedAbstractEvent;
    }
}
