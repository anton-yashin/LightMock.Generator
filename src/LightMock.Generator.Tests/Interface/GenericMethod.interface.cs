namespace LightMock.Generator.Tests.Interface
{
    public interface IGenericMethod
    {
        T GenericReturn<T>();
        void GenericParam<T>(T p);
        void GenericWithConstraint<T>(T? p) where T : class;
    }
}
