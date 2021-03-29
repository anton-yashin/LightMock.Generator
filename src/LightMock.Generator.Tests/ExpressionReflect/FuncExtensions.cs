#nullable disable
using System;
using System.Linq.Expressions;
using global::ExpressionReflect;

namespace LightMock.Generator.Tests.ExpressionReflect
{
    static class FuncExtensions
    {
		public static Func<TResult> Reflect<TResult>(this Expression<Func<TResult>> target)
		{
			Func<TResult> func = () => (TResult)target.Execute()!;
			return func;
		}

		public static Func<T, TResult> Reflect<T, TResult>(this Expression<Func<T, TResult>> target)
		{
			Func<T, TResult> func = a => (TResult)target.Execute(a);
			return func;
		}
	}
}
