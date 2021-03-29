using System;

namespace LightMock.Generator.Tests.ExpressionReflect.Model
{
	struct CustomStruct
	{
		private readonly int value;

        public CustomStruct(int value) => this.value = value;

        public int Value => this.value;
    }
}
