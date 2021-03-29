using LightMock.Generator.Tests.ExpressionReflect.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace LightMock.Generator.Tests.ExpressionReflect
{
    public class ActionExecutionTests
    {
		[Fact]
		public void ShouldCreateSimpleAction_NestedAction()
		{
			IList<string> list = new List<string>();
			list.Add("Hallo");
			list.Add("Welt");

			Expression<Action<IEnumerable<string>>> expression = x => x.ForEach(() => Console.WriteLine("Write"));
			Console.WriteLine(expression);

			Action<IEnumerable<string>> reflect = expression.Reflect();
			reflect.Invoke(list);
		}
	}
}
