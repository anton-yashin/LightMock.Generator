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
    public class UnaryOperatorTests
    {
		[Fact]
		public void ShouldExecuteOperator_Not()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe") { IsPremium = true };
			Expression<Func<Customer, bool>> expression = x => !x.IsPremium;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, bool> emit = expression.Compile();
			Func<Customer, bool> reflection = expression.Reflect();

			bool emitResult = emit.Invoke(customer);
			bool reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.False(emitResult);
			Assert.False(reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_Convert()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string>> expression = x => x.Firstname + "-" + 50;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "John-50", emitResult);
			Assert.Equal(expected: "John-50", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_ArrayLength()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.Names.Length;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 2, emitResult);
			Assert.Equal(expected: 2, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_TypeAs()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer?>> expression = x => x.Object as Customer;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer?> emit = expression.Compile();
			Func<Customer, Customer?> reflection = expression.Reflect();

			Customer? emitResult = emit.Invoke(customer);
			Customer? reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Null(emitResult);
			Assert.Null(reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}
	}
}
