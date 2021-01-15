namespace LightMock.Generator.Tests.Mock
{
    public interface ITestScript<T>
        where T : class
    {
        MockContext<T> Context { get; }
        T MockObject { get; }

        int DoRun();
    }
}
