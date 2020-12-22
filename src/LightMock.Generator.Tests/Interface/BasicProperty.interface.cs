namespace LightMock.Generator.Tests.Interface
{
    public interface IBasicProperty
    {
        int OnlyGet { get; }
        int GetAndSet { get; set; }
    }
}
