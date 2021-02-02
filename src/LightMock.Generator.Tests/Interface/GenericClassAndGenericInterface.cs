using System.Threading.Tasks;

namespace LightMock.Generator.Tests.Interface
{
    public interface IGenericClassAndGenericInterface<T>
    {
        T OnlyGet { get; }
        T GetAndSet { get; set; }
        T GetSomething();
        void DoSomething(T p);
        Task<TResult> FooAsync<TResult>() where TResult : class;
    }
}
