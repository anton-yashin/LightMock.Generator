using System;
using System.Runtime.Serialization;

namespace LightMock.Generator.Tests.TestAbstractions
{
    public sealed class ValidProgramException : Exception
    {
        public ValidProgramException()
        {
        }

        public ValidProgramException(string? message) : base(message)
        {
        }

        public ValidProgramException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ValidProgramException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
