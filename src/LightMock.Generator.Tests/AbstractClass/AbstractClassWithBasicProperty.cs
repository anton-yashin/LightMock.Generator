using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class AAbstractClassWithBasicProperty
    {
        public abstract int OnlyGet { get; }
        public abstract int GetAndSet { get; set; }

        public string NonAbstractNonVirtualProperty
            => throw new System.NotImplementedException();

        protected abstract int ProtectedOnlyGet { get; }
        protected abstract int ProtectedGetAndSet { get; set; }

        protected string ProtectedNonAbstractNonVirtualProperty
            => throw new System.NotImplementedException();

        public int InvokeProtectedOnlyGet => ProtectedOnlyGet;
        public int InvokeProtectedGetAndSet
        {
            get => ProtectedGetAndSet;
            set => ProtectedGetAndSet = value;
        }
    }
}
