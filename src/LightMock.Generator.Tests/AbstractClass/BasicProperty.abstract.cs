namespace LightMock.Generator.Tests.AbstractClass
{
    public abstract class ABasicProperty
    {
        public abstract int OnlyGet { get; }
        protected abstract int GetAndSet { get; set; }

        public string NonAbstractNonVirtualProperty
            => throw new System.NotImplementedException();
    }
}
