using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    public sealed class MockNotGeneratedException : Exception
    {
        public MockNotGeneratedException(Type type)
            : base(CreateMessage(type))
        {
        }

        static string CreateMessage(Type type)
        {
            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();
            return $@"Mock for {type.FullName} is not generated.
Try to place: “new Mock<{type.FullName}>();” somewhere into your project";
        }
    }
}
