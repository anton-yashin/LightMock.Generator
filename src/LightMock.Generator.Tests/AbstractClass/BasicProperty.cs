namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ABasicProperty
    {
        public abstract int OnlyGet { get; }
        public abstract int GetAndSet { get; set; }

        public string NonAbstractNonVirtualProperty
            => throw new System.NotImplementedException();

        protected abstract int ProtectedOnlyGet { get; }
        protected abstract int ProtectedGetAndSet { get; set; }

        protected string ProtectedNonAbstractNonVirtualProperty
            => throw new System.NotImplementedException();
    }
}
