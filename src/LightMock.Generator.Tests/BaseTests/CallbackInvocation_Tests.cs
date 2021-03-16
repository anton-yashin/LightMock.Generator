using System;
using Xunit;

namespace LightMock.Generator.Tests.BaseTests
{
    public class CallbackInvocation_Tests
    {
        [Theory, MemberData(nameof(NoMethodProvided_Data))]
        public void NoMethodProvided_Successful(object[]? prms)
        {
            var cb = new CallbackInvocation();
            cb.Invoke(prms);
        }

        [Theory, MemberData(nameof(NoMethodProvided_Data))]
        public void NoMethodProvided_ReturnsDefault(object[]? prms)
        {
            var cb = new CallbackInvocation();

            var actual = cb.Invoke<Guid>(prms);
            Assert.Equal(default(Guid), actual);
        }

        [Theory, MemberData(nameof(NoMethodProvided_Data))]
        public void InvokesActionWithoutParams(object[]? prms)
        {
            int invoked = 0;
            void SomeMethod() => invoked++;
            var cb = new CallbackInvocation();

            cb.Method = new Action(SomeMethod);
            cb.Invoke(prms);

            Assert.Equal(1, invoked);
        }

        [Theory, MemberData(nameof(NoMethodProvided_Data))]
        public void InvokesFunctionWithoutParams(object[]? prms)
        {
            var expected = Guid.NewGuid();
            int invoked = 0;
            Guid SomeMethod() { invoked++; return expected; }
            var cb = new CallbackInvocation();

            cb.Method = new Func<Guid>(SomeMethod);
            var actual = cb.Invoke<Guid>(prms);

            Assert.Equal(1, invoked);
            Assert.Equal(expected, actual);
        }

        public static TheoryData<object[]?> NoMethodProvided_Data()
            => new TheoryData<object[]?>()
            {
                null,
                Array.Empty<object>(),
                new string[] { "a", "b", "c" },
            };

        [Theory, MemberData(nameof(ParametersCountMismatch_Throws_Data))]
        public void ActionParametersCountMismatch_Throws(Type exceptionType, object[]? prms)
        {
            void SomeMethod(object a, object b) { }
            var cb = new CallbackInvocation();
            cb.Method = new Action<object, object>(SomeMethod);

            Assert.Throws(exceptionType, () => cb.Invoke(prms));
        }

        [Theory, MemberData(nameof(ParametersCountMismatch_Throws_Data))]
        public void FunctionParametersCountMismatch_Throws(Type exceptionType, object[]? prms)
        {
            string SomeMethod(object a, object b) => "zzz";
            var cb = new CallbackInvocation();
            cb.Method = new Func<object, object, string>(SomeMethod);

            Assert.Throws(exceptionType, () => cb.Invoke(prms));
        }

        public static TheoryData<Type, object[]?> ParametersCountMismatch_Throws_Data()
            => new TheoryData<Type, object[]?>()
            {
                { typeof(ArgumentNullException), null },
                { typeof(ArgumentException), Array.Empty<object>() },
                { typeof(ArgumentException), new string[] { "a" } },
                { typeof(ArgumentException), new string[] { "a", "b", "c" } },
            };

        [Fact]
        public void ReturnTypeMismatch_Throws()
        {
            CallbackInvocation SomeMethod() => null!;

            var cb = new CallbackInvocation();
            cb.Method = new Func<CallbackInvocation>(SomeMethod);

            Assert.Throws<ArgumentException>(() => cb.Invoke<string>(null));
        }
    }
}
