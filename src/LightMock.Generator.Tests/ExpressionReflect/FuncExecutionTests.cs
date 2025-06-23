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
    public class FuncExecutionTests
    {
		[Fact]
		public void ShouldCreateSimpleFunc_ReturnExpressionParameter()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => x;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.NotNull(emitResult);
			Assert.NotNull(reflectionResult);
			Assert.Same(emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_PropertyGetter()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string>> expression = x => x.Firstname;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "John", emitResult);
			Assert.Equal(expected: "John", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticPropertyGetter()
		{
			//Arrange
			Expression<Func<string>> expression = () => Customer.StaticProperty;

			//Act
			Func<string> emit = expression.Compile();
			Func<string> reflection = expression.Reflect();

			string emitResult = emit.Invoke();
			string reflectionResult = reflection.Invoke();

			//Assert
			Assert.Equal(expected: "StaticProperty", emitResult);
			Assert.Equal(expected: "StaticProperty", reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_StaticField()
		{
			//Arrange
			Expression<Func<string>> expression = () => Customer.StaticField;

			//Act
			Func<string> emit = expression.Compile();
			Func<string> reflection = expression.Reflect();

			string emitResult = emit.Invoke();
			string reflectionResult = reflection.Invoke();

			//Assert
			Assert.Equal(expected: "StaticField", emitResult);
			Assert.Equal(expected: "StaticField", reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_PropertyGetter_MethodCall()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string>> expression = x => x.Firstname.ToLower();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "john", emitResult);
			Assert.Equal(expected: "john", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}


		[Fact]
		public void ShouldCreateSimpleFunc_PropertyGetter_BinaryExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, bool>> expression = x => x.Firstname != "Jane";
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, bool> emit = expression.Compile();
			Func<Customer, bool> reflection = expression.Reflect();

			bool emitResult = emit.Invoke(customer);
			bool reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.True(emitResult);
			Assert.True(reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_PropertyGetter_UnaryExpression()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int arg = 10;
			Expression<Func<Customer, int>> expression = x => -arg;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: -10, emitResult);
			Assert.Equal(expected: -10, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_Indexer()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x[2];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 35, emitResult);
			Assert.Equal(expected: 35, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_Indexer_MultipleParameters()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			int arg = 5;
			Expression<Func<Customer, int>> expression = x => x[2, arg];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 40, emitResult);
			Assert.Equal(expected: 40, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_PropertyGetter_Indexer()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, char>> expression = x => x.Firstname[0];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, char> emit = expression.Compile();
			Func<Customer, char> reflection = expression.Reflect();

			char emitResult = emit.Invoke(customer);
			char reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 'J', emitResult);
			Assert.Equal(expected: 'J', reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_PropertyGetter_CustomIndexer()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, int>> expression = x => x.NameIndex["John"];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, int> emit = expression.Compile();
			Func<Customer, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(customer);
			int reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 0, emitResult);
			Assert.Equal(expected: 0, reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ArrayAccess()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string>> expression = x => x.Names[1];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "Doe", emitResult);
			Assert.Equal(expected: "Doe", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ArrayAccess_Local()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			string[] array = new string[]
			{
				"One", "Two"
			};
			Expression<Func<Customer, string>> expression = x => array[0];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "One", emitResult);
			Assert.Equal(expected: "One", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ArrayAccess_Delegate()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<string[]> func = () => new string[]
			{
				"One", "Two"
			};
			Expression<Func<Customer, string>> expression = x => func()[0];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "One", emitResult);
			Assert.Equal(expected: "One", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ArrayAccess_Delegate_CallingInvoke()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<string[]> func = () => new string[]
			{
				"One", "Two"
			};
			Expression<Func<Customer, string>> expression = x => func.Invoke()[0];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "One", emitResult);
			Assert.Equal(expected: "One", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_Delegate_CallingMethod()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<string> func = () => "Hello";
			Expression<Func<Customer, string>> expression = x => func().ToLower();
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "hello", emitResult);
			Assert.Equal(expected: "hello", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_Delegate_CallingMethod_WithParameter()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Func<string, string> func = x => "hello " + x;
			string s = "test";
			Expression<Func<Customer, string>> expression = x => func(s).Trim('h', 't');
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "ello tes", emitResult);
			Assert.Equal(expected: "ello tes", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ObjectInitializer()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer
			{
				Firstname = x.Firstname,
				Lastname = x.Lastname
			};
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "John", emitResult.Firstname);
			Assert.Equal(expected: "John", reflectionResult.Firstname);
			Assert.Equal(expected: "Doe", emitResult.Lastname);
			Assert.Equal(expected: "Doe", reflectionResult.Lastname);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ListInitializer()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, IList<string>>> expression = x => new List<string>
			{
				"Hello",
				"World"
			};
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, IList<string>> emit = expression.Compile();
			Func<Customer, IList<string>> reflection = expression.Reflect();

			IList<string> emitResult = emit.Invoke(customer);
			IList<string> reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 2, emitResult.Count);
			Assert.Equal(expected: "Hello", emitResult[0]);
			Assert.Equal(expected: 2, reflectionResult.Count);
			Assert.Equal(expected: "Hello", reflectionResult[0]);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_DictionaryInitializer()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, IDictionary<string, string>>> expression =
				x => new Dictionary<string, string>
				{
					{
						"1", "Hello"
					},
					{
						"2", "World"
					}
				};
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, IDictionary<string, string>> emit = expression.Compile();
			Func<Customer, IDictionary<string, string>> reflection = expression.Reflect();

			IDictionary<string, string> emitResult = emit.Invoke(customer);
			IDictionary<string, string> reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 2, emitResult.Count);
			Assert.Equal(expected: "Hello", emitResult["1"]);
			Assert.Equal(expected: 2, reflectionResult.Count);
			Assert.Equal(expected: "Hello", reflectionResult["1"]);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_CreateNewArray()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string[]>> expression = x => new string[]
			{
				"1", "2"
			};
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string[]> emit = expression.Compile();
			Func<Customer, string[]> reflection = expression.Reflect();

			string[] emitResult = emit.Invoke(customer);
			string[] reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "1", emitResult[0]);
			Assert.Equal(expected: "1", reflectionResult[0]);
			Assert.Equal(expected: emitResult[0], reflectionResult[0]);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_CreateNewArray_Bounds()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, string[]>> expression = x => new string[12];
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string[]> emit = expression.Compile();
			Func<Customer, string[]> reflection = expression.Reflect();

			string[] emitResult = emit.Invoke(customer);
			string[] reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: 12, emitResult.Length);
			Assert.Equal(expected: 12, reflectionResult.Length);
			Assert.Equal(expected: emitResult.Length, reflectionResult.Length);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_NestedExpression_ReferenceParameter_ValueResult()
		{
			// Arrange
			IList<Customer> list = new List<Customer>();
			list.Add(new Customer("John", "Doe"));
			list.Add(new Customer("Jane", "Doe"));

			Expression<Func<IList<Customer>, Customer?>> expression = a => a.FirstOrDefault(x => x.Lastname == "Doe");
			Console.WriteLine(expression.ToString());

			// Act
			Func<IList<Customer>, Customer?> emit = expression.Compile();
			Func<IList<Customer>, Customer?> reflection = expression.Reflect();

			Customer? emitResult = emit.Invoke(list);
			Customer? reflectionResult = reflection.Invoke(list);

			// Assert
			Assert.Equal(expected: "John", emitResult?.Firstname);
			Assert.Equal(expected: "John", reflectionResult?.Firstname);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_NestedExpression_ValueParameter_ValueResult()
		{
			// Arrange
			IList<int> list = new List<int>();
			list.Add(50);
			list.Add(100);

			Expression<Func<IList<int>, int>> expression = a => a.FirstOrDefault(x => x == 50);
			Console.WriteLine(expression.ToString());

			// Act
			Func<IList<int>, int> emit = expression.Compile();
			Func<IList<int>, int> reflection = expression.Reflect();

			int emitResult = emit.Invoke(list);
			int reflectionResult = reflection.Invoke(list);

			// Assert
			Assert.Equal(expected: 50, emitResult);
			Assert.Equal(expected: 50, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_NestedExpression_ValueParameter_CustomStructResult()
		{
			// Arrange
			IList<CustomStruct> list = new List<CustomStruct>();
			list.Add(new CustomStruct(50));
			list.Add(new CustomStruct(100));

			Expression<Func<IList<CustomStruct>, CustomStruct>> expression = a => a.FirstOrDefault(x => x.Value == 50);
			Console.WriteLine(expression.ToString());

			// Act
			Func<IList<CustomStruct>, CustomStruct> emit = expression.Compile();
			Func<IList<CustomStruct>, CustomStruct> reflection = expression.Reflect();

			CustomStruct emitResult = emit.Invoke(list);
			CustomStruct reflectionResult = reflection.Invoke(list);

			// Assert
			Assert.Equal(expected: 50, emitResult.Value);
			Assert.Equal(expected: 50, reflectionResult.Value);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_NestedExpression_TwoParameters()
		{
			// Arrange
			IList<Customer> list = new List<Customer>();
			list.Add(new Customer("John", "Doe"));
			list.Add(new Customer("Jane", "Doe"));

			Expression<Func<IList<Customer>, Customer>> expression = a => a.FirstOrDefaultCustom((x, y) => x.Lastname == "Doe");
			Console.WriteLine(expression.ToString());

			// Act
			Func<IList<Customer>, Customer> emit = expression.Compile();
			Func<IList<Customer>, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(list);
			Customer reflectionResult = reflection.Invoke(list);

			// Assert
			Assert.Equal(expected: "John", emitResult.Firstname);
			Assert.Equal(expected: "John", reflectionResult.Firstname);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_Field()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			customer.Field = "SoylentGreen";
			Expression<Func<Customer, string>> expression = x => x.Field;
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, string> emit = expression.Compile();
			Func<Customer, string> reflection = expression.Reflect();

			string emitResult = emit.Invoke(customer);
			string reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "SoylentGreen", emitResult);
			Assert.Equal(expected: "SoylentGreen", reflectionResult);
			Assert.Equal(expected: emitResult, reflectionResult);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_ObjectInitializer_WithField()
		{
			// Arrange
			Customer customer = new Customer("John", "Doe");
			Expression<Func<Customer, Customer>> expression = x => new Customer
			{
				Field = "SoylentGreen",
			};
			Console.WriteLine(expression.ToString());

			// Act
			Func<Customer, Customer> emit = expression.Compile();
			Func<Customer, Customer> reflection = expression.Reflect();

			Customer emitResult = emit.Invoke(customer);
			Customer reflectionResult = reflection.Invoke(customer);

			// Assert
			Assert.Equal(expected: "SoylentGreen", emitResult.Field);
			Assert.Equal(expected: "SoylentGreen", reflectionResult.Field);
		}

		[Fact]
		public void ShouldCreateSimpleFunc_NestedLambda_WithEnumeration()
		{
			//Arrange
			Expression<Func<string, IEnumerable<char>>> expression = (p) => p.Where((c) => c == 'S').ToList();

			//Act
			Func<string, IEnumerable<char>> emit = expression.Compile();
			Func<string, IEnumerable<char>> reflection = expression.Reflect();

			IEnumerable<char> emitResult = emit.Invoke("SomeString");
			IEnumerable<char> reflectionResult = reflection.Invoke("SomeString");

			// Assert
			Assert.Equal(['S', 'S'], emitResult);
			Assert.Equal(['S', 'S'], reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}


		[Fact]
		public void ShouldCreateSimpleFunc_NestedChainedLambda_WithEnumeration()
		{
			//Arrange
			Expression<Func<string, IEnumerable<char>>> expression =
				(p) => p.Where((c) => c == 'S').Select(c => char.ToLower(c)).ToList();

			//Act
			Func<string, IEnumerable<char>> emit = expression.Compile();
			Func<string, IEnumerable<char>> reflection = expression.Reflect();

			IEnumerable<char> emitResult = emit.Invoke("SomeString");
			IEnumerable<char> reflectionResult = reflection.Invoke("SomeString");

			// Assert
			Assert.Equal(['s', 's'], emitResult);
			Assert.Equal(['s', 's'], reflectionResult);
			Assert.Equal(emitResult, reflectionResult);
		}
	}
}
