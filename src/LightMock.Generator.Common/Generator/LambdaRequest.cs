using System;
using System.Linq.Expressions;

namespace LightMock.Generator
{
    sealed class LambdaRequest : ILambdaRequest
    {
        public LambdaExpression? Result { get; private set; }

        public void SetResult(LambdaExpression result) => Result = result;
    }
}
