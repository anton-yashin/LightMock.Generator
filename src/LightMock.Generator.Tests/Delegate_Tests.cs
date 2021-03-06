﻿using LightMock.Generator.Tests.Delegate;
using LightMock.Generator.Tests.TestAbstractions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace LightMock.Generator.Tests
{
    public class Delegate_Tests : GeneratorTestsBase
    {
        const int KExpected = 42;

        public Delegate_Tests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Fact]
        public void BasicDelegate()
        {
            var expectedObject = new object();
            var expectedEventArgs = new EventArgs();
            var testScript = LoadAssembly<EventHandler>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock(expectedObject, expectedEventArgs);
            context.Assert(f => f.Invoke(expectedObject, expectedEventArgs));
        }

        [Fact]
        public void GenericDelegate()
        {
            var (e1, e2, e3, er) = (new object(), 1234, 5678L, 9012L);

            var testScript = LoadAssembly<SomeGenericDelegate<object, int, long, long>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            context.Arrange(f => f.Invoke(The<object>.IsAnyValue, The<int>.IsAnyValue, The<long>.IsAnyValue)).Returns(er);
            var result = mock(e1, e2, e3);
            Assert.Equal(er, result);
            context.Assert(f => f.Invoke(e1, e2, e3));
        }

        [Fact]
        public void DelegateNotGenerated()
        {
            var testScript = LoadAssembly<EventHandler>();

            Assert.Throws<MockNotGeneratedException>(() => testScript.MockObject);
        }

        [Fact]
        public void NestedDelegate()
        {
            object expected = new object();
            var testScript = LoadAssembly<XNestedDelegate.SomeDelegate>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock(expected, 1234);
            context.Assert(f => f.Invoke(expected, 1234));
        }

        [Fact]
        public void NestedDelegateWithGenerics()
        {
            object expected = new object();
            var testScript = LoadAssembly<XNestedInterface<int>.XContainer<long>.SomeDelegate<object>>();
            var context = testScript.Context;
            var mock = testScript.MockObject;

            mock(1234, 4567, expected);
            context.Assert(f => f.Invoke(1234, 4567, expected));
        }

        protected override string GetFullResourceName(string resourceName)
            => "Delegate." + resourceName + ".test.cs";
    }
}
