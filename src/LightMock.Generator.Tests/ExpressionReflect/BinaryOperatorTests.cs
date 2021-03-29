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
    public class BinaryOperatorTests
    {
		[Fact]
		public void ShouldExecuteOperator_Add()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => 33 + 10;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 43, emitResult);
			Assert.Equal(expected: 43, reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_AddChecked()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => checked(33 + 10);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 43, emitResult);
			Assert.Equal(expected: 43, reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_TypeIs()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, bool>> expression = x => x is Customer;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, bool> emit = expression.Compile();
			Func<Customer, bool> reflection = expression.Reflect();

			bool emitResult = emit.Invoke(customer);
			bool reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.True(emitResult);
			Assert.True(reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_Equals()
		{
			// Arrange
			Expression<Func<string, bool>> expression = s => s == "SomeValue";

			// Act
			Func<string, bool> emit = expression.Compile();
			Func<string, bool> reflection = expression.Reflect();

			bool emitResult = emit.Invoke("SomeValue");
			bool reflectionResult = reflection.Invoke("SomeValue");

			// Assert
			Assert.True(emitResult);
			Assert.True(reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldExecuteOperator_EqualsWithNullValue()
		{
			// Arrange
			Expression<Func<string?, bool>> expression = s => s == null;

			// Act
			Func<string?, bool> emit = expression.Compile();
			Func<string?, bool> reflection = expression.Reflect();

			bool emitResult = emit.Invoke(null);
			bool reflectionResult = reflection.Invoke(null);

			// Assert
			Assert.True(emitResult);
			Assert.True(reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}
	}
}
