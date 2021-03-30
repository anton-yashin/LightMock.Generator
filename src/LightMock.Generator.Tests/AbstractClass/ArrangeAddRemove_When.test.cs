using LightMock.Generator.Tests.TestAbstractions;
using System;
using Xunit;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class ArrangeAddRemove_When : ITestScript<AArrangeAddRemove_When>
    {
        private readonly Mock<AArrangeAddRemove_When> mock;

        public ArrangeAddRemove_When() => mock = new Mock<AArrangeAddRemove_When>();

        public IMock<AArrangeAddRemove_When> Context => mock;

        public AArrangeAddRemove_When MockObject => mock.Object;

        public int DoRun() 
        {
            mock.Protected().ArrangeAdd_When(f => f.ProtectedEventHandler += SomeEventHandler).Throws<ValidProgramException>();
            mock.Protected().ArrangeRemove_When(f => f.ProtectedEventHandler -= SomeEventHandler).Throws<ValidProgramException>();

            Assert.Throws<ValidProgramException>(() => MockObject.InvokeProtectedEventHandler += SomeEventHandler);
            Assert.Throws<ValidProgramException>(() => MockObject.InvokeProtectedEventHandler -= SomeEventHandler);

            MockObject.InvokeProtectedEventHandler += AnotherEventHandler;
            MockObject.InvokeProtectedEventHandler -= AnotherEventHandler;

            return 42;

            void SomeEventHandler(object? o, EventArgs a) { }
            void AnotherEventHandler(object? o, EventArgs a) { }
        }
    }
}
