namespace StaticProxy.Tests.Testee
{
    public interface IBasicProperty
    {
        int OnlyGet { get; }
        int GetAndSet { get; set; }
    }
}
