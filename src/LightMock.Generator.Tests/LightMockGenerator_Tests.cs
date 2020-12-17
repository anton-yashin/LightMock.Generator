using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using LightMock.Generator.Tests.Testee;
using Xunit;
using Xunit.Abstractions;
using MultipleNamespaces2;
using MultipleNamespaces1;

namespace LightMock.Generator.Tests
{
    public class LightMockGenerator_Tests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public LightMockGenerator_Tests(ITestOutputHelper testOutputHelper)
            => this.testOutputHelper = testOutputHelper;

        [Fact]
        public void BasicMethod()
        {
            const string KClassName = "BasicMethod";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IBasicMethod>(KClassName, assembly);

            @interface.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));
            
            context.Arrange(f => f.ReturnSomething()).Returns(5678);
            Assert.Equal(5678, @interface.ReturnSomething());
        }

        [Fact]
        public void BasicProperty()
        {
            const string KClassName = "BasicProperty";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IBasicProperty>(KClassName, assembly);

            context.Arrange(f => f.OnlyGet).Returns(1234);
            Assert.Equal(1234, @interface.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            @interface.GetAndSet = 5678;
            Assert.Equal(5678, @interface.GetAndSet);
        }

        [Fact]
        public void GenericMethod()
        {
            const string KClassName = "GenericMethod";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IGenericMethod>(KClassName, assembly);

            context.Arrange(f => f.GenericReturn<int>()).Returns(1234);
            Assert.Equal(1234, @interface.GenericReturn<int>());

            @interface.GenericParam<int>(5678);
            context.Assert(f => f.GenericParam<int>(5678));

            var p = new object();
            @interface.GenericWithConstraint(p);
            context.Assert(f => f.GenericWithConstraint(p));
        }

        [Fact]
        public void GenericClassAndGenericInterface()
        {
            const string KClassName = "GenericClassAndGenericInterface";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IGenericClassAndGenericInterface<int>>(KClassName + "`1", assembly);

            @interface.DoSomething(1234);
            context.Assert(f => f.DoSomething(1234));

            context.Arrange(f => f.GetSomething()).Returns(5678);
            Assert.Equal(5678, @interface.GetSomething());

            context.Arrange(f => f.OnlyGet).Returns(9012);
            Assert.Equal(9012, @interface.OnlyGet);

            context.ArrangeProperty(f => f.GetAndSet);
            @interface.GetAndSet = 3456;
            Assert.Equal(3456, @interface.GetAndSet);
        }

        [Fact]
        public void MultipleNamespaces()
        {
            const string KClassName = "MultipleNamespaces";
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource(KClassName + ".class.cs"));

            // verify
            Assert.True(success);
            Assert.Empty(diagnostics);

            var (context, @interface) = LoadAssembly<IMultipleNamespaces>(KClassName, assembly);

            var arg1 = new MultipleNamespacesArgument();
            @interface.DoSomething(arg1);
            context.Assert(f => f.DoSomething(arg1));

            var arg2 = new MultipleNamespacesArgument();
            context.Arrange(f => f.GetSomething()).Returns(arg2);
            Assert.Same(expected: arg2, @interface.GetSomething());

            var arg3 = new MultipleNamespacesArgument();
            context.ArrangeProperty(f => f.SomeProperty);
            @interface.SomeProperty = arg3;
            Assert.Same(expected: arg3, @interface.SomeProperty);
        }

        [Fact]
        public void NoPartialKeyworkError()
        {
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource("NoPartialKeyworkError.class.cs"));

            // verify
            Assert.False(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG002");
        }

        [Fact]
        public void NoInterfaceError()
        {
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource("NoInterfaceError.class.cs"));

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG003");
        }

        [Fact]
        public void TooManyInterfacesWarning()
        {
            var (diagnostics, success, assembly) = DoCompile(Utils.LoadResource("TooManyInterfacesWarning.class.cs"));

            // verify
            Assert.True(success);
            Assert.Contains(diagnostics, d => d.Id == "SPG004");
        }

        private static (MockContext<T>, T) LoadAssembly<T>(string className, byte[] assembly)
        {
            var alc = new AssemblyLoadContext(className);
            var loadedAssembly = alc.LoadFromStream(new MemoryStream(assembly));
            var concrete = loadedAssembly.ExportedTypes.Where(t => t.Name == className).First();
            if (concrete.ContainsGenericParameters)
                concrete = concrete.MakeGenericType(typeof(T).GetGenericArguments().First());
            var context = new MockContext<T>();
            var mockInstance = Activator.CreateInstance(concrete, context) ?? throw new InvalidOperationException("can't create instance");
            var @interface = (T)mockInstance;
            return (context, @interface);
        }

        private (ImmutableArray<Diagnostic> diagnostics, bool succes, byte[] assembly) DoCompile(string source)
        {
            var compilation = CreateCompilation(source);
            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(new LightMockGenerator()),
                Enumerable.Empty<AdditionalText>(),
                (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
            var ms = new MemoryStream();
            var result = updatedCompilation.Emit(ms);
            foreach (var i in result.Diagnostics)
                testOutputHelper.WriteLine(i.ToString());
            return (diagnostics, result.Success, ms.ToArray());
        }


        private static CSharpCompilation CreateCompilation(string source, string? compilationName = null)
            => CSharpCompilation.Create(compilationName ?? Guid.NewGuid().ToString("N"),
                syntaxTrees: new[]
                {
                    CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))
                },
                references: new[]
                {
                    MetadataReference.CreateFromFile(Assembly.GetCallingAssembly().Location),
                    MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(LightMock.InvocationInfo).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Linq.Expressions")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                },
                options: new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }
}
