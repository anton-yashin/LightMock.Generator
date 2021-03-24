using LightMock.Generator.Tests.TestAbstractions;
using System;

namespace LightMock.Generator.Tests.AbstractClass
{
    public class AssertNoOtherCalls_Throws : ITestScript<AAssertNoOtherCalls_Throws>
    {
        private readonly Mock<AAssertNoOtherCalls_Throws> mock;

        public AssertNoOtherCalls_Throws() => mock = new Mock<AAssertNoOtherCalls_Throws>();

        public IMock<AAssertNoOtherCalls_Throws> Context => mock;

        public AAssertNoOtherCalls_Throws MockObject => mock.Object;

        public int DoRun()
        {
            mock.Protected().AssertSet_When(f => f.ProtectedGetAndSet = nameof(AAssertNoOtherCalls.GetAndSet));
            mock.Protected().AssertSet_When(f => f.ProtectedSetOnly = nameof(AAssertNoOtherCalls.SetOnly));
            mock.Protected().AssertSet_When(f => f["123"] = "indexer_set");
            mock.Protected().AssertGet(f => f.ProtectedGetAndSet);
            mock.Protected().AssertGet(f => f.ProtectedGetOnly);
            mock.Protected().AssertGet(f => f["456"]);
            mock.Protected().Assert(f => f.ProtectedFunction(nameof(AAssertNoOtherCalls.Function)));
            mock.Protected().Assert(f => f.ProtectedMethod(nameof(AAssertNoOtherCalls.Method)));

            return 42;
        }
    }
}
