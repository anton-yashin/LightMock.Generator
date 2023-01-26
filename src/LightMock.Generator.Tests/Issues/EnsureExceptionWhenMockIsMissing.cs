namespace LightMock.Generator.Tests.Issues
{
    public interface IEnsureExceptionWhenMockIsMissingBase
    {
        void Foo();
    }

    public interface IEnsureExceptionWhenMockIsMissing : IEnsureExceptionWhenMockIsMissingBase
    {
        void Bar();
    }

}
