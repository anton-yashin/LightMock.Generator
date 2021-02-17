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
            var tn = type.FullName.Replace("+", ".");
            return $@"Mock for {tn} is not generated.
Try to place: “new Mock<{tn}>();” somewhere into your project";
        }
    }
}
