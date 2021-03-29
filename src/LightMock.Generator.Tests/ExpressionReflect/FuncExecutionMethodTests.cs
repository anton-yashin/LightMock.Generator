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
    public class FuncExecutionMethodTests
    {
		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.CalculateAge();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: Customer.AgeConstant, emitResult);
			Assert.Equal(expected: Customer.AgeConstant, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.CalculateLength(x.Firstname);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 4, emitResult);
			Assert.Equal(expected: 4, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_BinaryExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.Calculate(x.Age + x.Value);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 76, emitResult);
			Assert.Equal(expected: 76, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_BinaryExpression_ConstantExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.Calculate(x.Age + 100);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 166, emitResult);
			Assert.Equal(expected: 166, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_BinaryExpression_LocalVariable()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int value = 666;
			Expression<Func<Customer, int>> expression = x => x.Calculate(value);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 699, emitResult);
			Assert.Equal(expected: 699, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_NestedNew()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int value = 666;
			Expression<Func<Customer, int>> expression = x => x.Calculate(new Customer(value));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 699, emitResult);
			Assert.Equal(expected: 699, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_NestedMethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.Calculate(x.CalculateAge());
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 66, emitResult);
			Assert.Equal(expected: 66, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_LocalDelegateCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int> method = () => 100;
			Expression<Func<Customer, int>> expression = x => x.Calculate(method());
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 133, emitResult);
			Assert.Equal(expected: 133, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_NestedDelegateCall_WithParameters_Constant()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int, int> method = x => x + 100;
			Expression<Func<Customer, int>> expression = x => x.Calculate(method(10));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 143, emitResult);
			Assert.Equal(expected: 143, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_NestedDelegateCall_WithParameters_Local()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int, int> method = x => x + 100;
			int arg = 10;
			Expression<Func<Customer, int>> expression = x => x.Calculate(method(arg));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 143, emitResult);
			Assert.Equal(expected: 143, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithExpressionParameters_NestedDelegateCall_WithParameters_BinaryExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int, int> method = x => x + 100;
			Expression<Func<Customer, int>> expression = x => x.Calculate(method(5 + 5));
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 143, emitResult);
			Assert.Equal(expected: 143, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_MethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string>> expression = x => x.ToString().ToLower();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "john doe", emitResult);
			Assert.Equal(expected: "john doe", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticMethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => Customer.GetDefaultAge();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: Customer.AgeConstant, emitResult);
			Assert.Equal(expected: Customer.AgeConstant, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticMethodCall_Constant()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => Customer.GetDefaultAge(20);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 53, emitResult);
			Assert.Equal(expected: 53, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticMethodCall_Local()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int value = 20;
			Expression<Func<Customer, int>> expression = x => Customer.GetDefaultAge(value);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 53, emitResult);
			Assert.Equal(expected: 53, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticMethodCall_Delegate()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int> value = () => 20;
			Expression<Func<Customer, int>> expression = x => Customer.GetDefaultAge(value());
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 53, emitResult);
			Assert.Equal(expected: 53, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticMethodCall_ExpressionParameter()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => Customer.GetDefaultAge(x);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 66, emitResult);
			Assert.Equal(expected: 66, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ExtMethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.GetDefaultAgeEx();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 66, emitResult);
			Assert.Equal(expected: 66, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ExtMethodCall_Constant()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.GetDefaultAgeEx(20);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 53, emitResult);
			Assert.Equal(expected: 53, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ExtMethodCall_Local()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int value = 20;
			Expression<Func<Customer, int>> expression = x => x.GetDefaultAgeEx(value);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 53, emitResult);
			Assert.Equal(expected: 53, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ExtMethodCall_Delegate()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<int> value = () => 20;
			Expression<Func<Customer, int>> expression = x => x.GetDefaultAgeEx(value());
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 53, emitResult);
			Assert.Equal(expected: 53, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ExtMethodCall_ExpressionParameter()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.GetDefaultAgeEx();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 66, emitResult);
			Assert.Equal(expected: 66, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_MethodCall_WithMixedParameters()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.CalculateLength(x.Firstname, x, 10);
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 14, emitResult);
			Assert.Equal(expected: 14, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}
	}
}
