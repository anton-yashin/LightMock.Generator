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
    public class FuncExecutionConstructorTests
    {
		[Fact]
		public void ShouldCreateSimpleFunc_New()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.NotNull(emitResult);
			Assert.NotNull(reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer(x.Lastname, x.Firstname);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.NotNull(emitResult);
			Assert.NotNull(reflectionResult);
			Assert.Equal(expected: emitResult.Firstname, reflectionResult.Firstname);
			Assert.Equal(expected: emitResult.Lastname, reflectionResult.Lastname);
			Assert.Equal(expected: "Doe", reflectionResult.Firstname);
			Assert.Equal(expected: "John", reflectionResult.Lastname);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_BinaryExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer(x.Age + x.Value);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 43, emitResult.CalculationValue);
			Assert.Equal(expected: 43, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_BinaryExpression_ConstantExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer(x.Age + 100);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 133, emitResult.CalculationValue);
			Assert.Equal(expected: 133, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_BinaryExpression_LocalVariable()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int value = 666;
			Expression<Func<Customer, Customer>> expression = x => new Customer(value);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 666, emitResult.CalculationValue);
			Assert.Equal(expected: 666, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_NestedNew()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int value = 666;
			Expression<Func<Customer, Customer>> expression = x => new Customer(new Customer(value));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 666, emitResult.CalculationValue);
			Assert.Equal(expected: 666, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_NestedMethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer(x.CalculateAge());
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 33, emitResult.CalculationValue);
			Assert.Equal(expected: 33, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_NestedDelegateCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int> method = () => 100;
			Expression<Func<Customer, Customer>> expression = x => new Customer(method());
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 100, emitResult.CalculationValue);
			Assert.Equal(expected: 100, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_NestedDelegateCall_WithParameters_Constant()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int, int> method = x => x + 100;
			Expression<Func<Customer, Customer>> expression = x => new Customer(method(10));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 110, emitResult.CalculationValue);
			Assert.Equal(expected: 110, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_NestedDelegateCall_WithParameters_Local()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int, int> method = x => x + 100;
			int arg = 10;
			Expression<Func<Customer, Customer>> expression = x => new Customer(method(arg));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 110, emitResult.CalculationValue);
			Assert.Equal(expected: 110, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithExpressionParameters_NestedDelegateCall_WithParameters_BinaryExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int, int> method = x => x + 100;
			Expression<Func<Customer, Customer>> expression = x => new Customer(method(5 + 5));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 110, emitResult.CalculationValue);
			Assert.Equal(expected: 110, reflectionResult.CalculationValue);
			Assert.Equal(expected: emitResult.CalculationValue, reflectionResult.CalculationValue);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_MethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => new Customer().CalculateAge();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 33, emitResult);
			Assert.Equal(expected: 33, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_New_WithMixedParameters()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer(x.Lastname, x, 10, x.Firstname);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.NotNull(emitResult);
			Assert.NotNull(reflectionResult);
			Assert.Equal(expected: emitResult.Firstname, reflectionResult.Firstname);
			Assert.Equal(expected: emitResult.Lastname, reflectionResult.Lastname);
			Assert.Equal(expected: "Doe", reflectionResult.Firstname);
			Assert.Equal(expected: "John", reflectionResult.Lastname);
			Assert.Equal(expected: 43, reflectionResult.CalculationValue);
		}
	}
}
