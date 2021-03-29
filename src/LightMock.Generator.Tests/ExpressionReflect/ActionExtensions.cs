#nullable disable
using System;
using System.Linq.Expressions;
using global::ExpressionReflect;

namespace LightMock.Generator.Tests.ExpressionReflect
{
    static class ActionExtensions
    {
		public static Action Reflect(this Expression<Action> target)
		{
			Action action = () => target.Execute();
			return action;
		}

		public static Action<T> Reflect<T>(this Expression<Action<T>> target)
		{
			Action<T> action = a => target.Execute(a);
			return action;
		}
	}
}
