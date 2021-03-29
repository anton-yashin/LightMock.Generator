using LightMock.Generator.Tests.ExpressionReflect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using global::ExpressionReflect;

namespace LightMock.Generator.Tests.ExpressionReflect
{
    public class EvaluatorTests
    {
        private string GetString() => "test";

        private static string GetStringStatic() => "test";

        private string GetString(int i) => "test" + i;

        private static string GetStringStatic(int i) => "test" + i;

        [Fact]
		public void ShouldPreEvaluate_LocalVariable()
		{
			// Arrange
			string str = "test";
			Expression<Func<Customer, bool>> expression = x => x.Firstname == str;

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_LocalFuncWithoutParameter()
		{
			// Arrange
			Func<string> func = () => "test";
			Expression<Func<Customer, bool>> expression = x => x.Firstname == func();

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_LocalFuncWithConstantParameter()
		{
			// Arrange
			Func<int, string> func = x => "test" + x;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == func(5);

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_LocalFuncWithLocalVariableParameter()
		{
			// Arrange
			Func<int, string> func = x => "test" + x;
			int i = 5;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == func(i);

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_LocalFuncWithLocalFuncParameter()
		{
			// Arrange
			Func<int, string> func = x => "test" + x;
			Func<int> i = () => 5;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == func(i());

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_InstanceMethodCallWithoutParameter()
		{
			// Arrange
			Expression<Func<Customer, bool>> expression = x => x.Firstname == this.GetString();

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_StaticMethodCallWithoutParameter()
		{
			// Arrange
			Expression<Func<Customer, bool>> expression = x => x.Firstname == GetStringStatic();

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_InstanceMethodCallWithConstantParameter()
		{
			// Arrange
			Expression<Func<Customer, bool>> expression = x => x.Firstname == this.GetString(5);

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_StaticMethodCallWithConstantParameter()
		{
			// Arrange
			Expression<Func<Customer, bool>> expression = x => x.Firstname == GetStringStatic(5);

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_InstanceMethodCallWithLocalVariableParameter()
		{
			// Arrange
			int i = 5;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == this.GetString(i);

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_StaticMethodCallWithLocalVariableParameter()
		{
			// Arrange
			int i = 5;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == GetStringStatic(i);

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_InstanceMethodCallWithLocalFuncParameter()
		{
			// Arrange
			Func<int> i = () => 5;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == this.GetString(i());

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

		[Fact]
		public void ShouldPreEvaluate_StaticMethodCallWithLocalFuncParameter()
		{
			// Arrange
			Func<int> i = () => 5;
			Expression<Func<Customer, bool>> expression = x => x.Firstname == GetStringStatic(i());

			// Act
			var expressionString = expression.PartialEval()?.ToString();
			Console.WriteLine(expressionString);

			// Assert
			Assert.Equal(expected: @"x => (x.Firstname == ""test5"")", expressionString);
		}

	}
}
