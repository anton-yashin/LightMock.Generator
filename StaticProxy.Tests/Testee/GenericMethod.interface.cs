namespace StaticProxy.Tests.Testee
{
    public interface IGenericMethod
    {
        T GenericReturn<T>();
        void GenericParam<T>(T p);
        void GenericWithConstraint<T>(T p) where T : new();
    }
}
