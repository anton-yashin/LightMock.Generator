using LightMock.Generator.Tests.ExpressionReflect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LightMock.Generator.Tests.ExpressionReflect
{
    public class ConditionalTests
    {
		[Fact]
		public void ShouldExecuteConditional()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe") { IsPremium = true };
			Expression<Func<Customer, int>> expression = x => x.IsPremium ? 500 : 300;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 500, emitResult);
			Assert.Equal(expected: 500, reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}
	}
}
