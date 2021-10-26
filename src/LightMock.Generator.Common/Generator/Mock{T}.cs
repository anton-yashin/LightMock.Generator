using System;

namespace LightMock.Generator
{
    /// <summary>
    /// Class for mocks and mocked objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Mock<T> : AbstractMock<T>
        where T : class
    {
        /// <summary>
        /// Create mock and mocked object without paramters
        /// </summary>
        public Mock()
            : base(Array.Empty<object>())
        { }

        /// <summary>
        /// Create mock and mocked object with parameters
        /// </summary>
        /// <param name="prms">Parameters that passed into constructor of mocked object</param>
        public Mock(params object[] prms)
            : base(prms)
        { }
    }
}
