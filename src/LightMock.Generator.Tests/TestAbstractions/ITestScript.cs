namespace LightMock.Generator.Tests.TestAbstractions
{
    public interface ITestScript<T>
        where T : class
    {
        IMock<T> Context { get; }
        T MockObject { get; }

        int DoRun();
    }
}
