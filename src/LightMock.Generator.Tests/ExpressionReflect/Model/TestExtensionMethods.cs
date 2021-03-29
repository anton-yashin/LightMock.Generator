using System;
using System.Collections.Generic;

namespace LightMock.Generator.Tests.ExpressionReflect.Model
{
	public static class TestExtensionMethods
	{
		public static TSource FirstOrDefaultCustom<TSource>(this IEnumerable<TSource> source,
			Func<TSource, int, bool> predicate)
		{
			foreach (TSource obj in source)
			{
				if (predicate(obj, 50))
				{
					return obj;
				}
			}
			return default!;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action action)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (action == null) throw new ArgumentNullException("action");

			foreach (T item in source)
			{
				action();
			}
		}
	}
}
