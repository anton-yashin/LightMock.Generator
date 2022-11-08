using System;
using System.Collections.Generic;
using Xunit;

namespace LightMock.Generator.Tests.BaseTests
{
    public class ArgumentHelper_Tests
    {
        [Fact]
        public void Unpack_UsePacked()
        {
            const string ARG_NAME = nameof(ARG_NAME);
            var expected = new object();
            var args = new Dictionary<string, object>()
            {
                {ARG_NAME, expected},
            };

            var result = ArgumentHelper.Unpack<object>(args, ARG_NAME);
            Assert.Same(expected, result);
        }

        [Fact]
        public void Unpack_UseDefault()
        {
            const string ARG_NAME = nameof(ARG_NAME);
            var args = new Dictionary<string, object>();

            var result = ArgumentHelper.Unpack<object>(args, ARG_NAME);
            Assert.Null(result);
        }

        [Fact]
        public void Unpack_UseExplicitDefault()
        {
            const string ARG_NAME = nameof(ARG_NAME);
            object expected = new object();
            var args = new Dictionary<string, object>();

            var result = ArgumentHelper.Unpack<object>(args, ARG_NAME, expected);
            Assert.Same(expected, result);
        }

    }
}
