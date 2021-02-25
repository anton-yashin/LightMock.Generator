using System;
using System.Linq.Expressions;

namespace LightMock.Generator
{
    /// <summary>
    /// Use this class with nameof operator
    /// </summary>
    internal sealed class AbstractMockNameofProvider : AbstractMock<IDelegateProvider>
    {
        protected override Expression<Action<IDelegateProvider>> ExchangeForExpression(string token)
            => throw new NotImplementedException();

        protected override Type GetAssertType()
            => throw new NotImplementedException();

        protected override IDelegateProvider GetDelegate(Type type)
            => throw new NotImplementedException();

        protected override Type GetInstanceType()
            => throw new NotImplementedException();

        protected override Type GetPropertiesContextType()
            => throw new NotImplementedException();

        protected override Type GetProtectedContextType()
            => throw new NotImplementedException();
    }
}
