using System;

namespace LightMock.Generator
{
    public interface IMock<T> : IMockContext<T>
        where T : class
    {
        T Object { get; }

        void AssertGet<TResult>(Func<T, TResult> getterExpression);
        void AssertGet<TResult>(Func<T, TResult> getterExpression, Invoked invoked);
        void AssertSet(Action<T> setterExpression);
        void AssertSet(Action<T> setterExpression, Invoked invoked);
    }
}
