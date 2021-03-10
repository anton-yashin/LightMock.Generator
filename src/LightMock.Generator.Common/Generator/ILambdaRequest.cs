using System;
using System.Linq.Expressions;

namespace LightMock.Generator
{
    public interface ILambdaRequest
    {
        void SetResult(LambdaExpression result);
    }
}
