/******************************************************************************
    MIT License

    Copyright (c) 2021 Anton Yashin

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*******************************************************************************
    https://github.com/anton-yashin/
*******************************************************************************/
using System;
using System.Diagnostics.CodeAnalysis;

namespace LightMock
{
    sealed class CallbackInvocation
    {
        private Delegate? __method;
        private readonly object __methodLock = new object();

        public CallbackInvocation() { }

        public Delegate? Method
        { 
            get
            {
                lock (__methodLock)
                    return __method;
            }
            set
            {
                lock (__methodLock)
                    __method = value;
            }
        }

        public void Invoke(object[]? parameters)
        {
            var method = Method;
            if (method != null)
            {
                parameters = ValidateMethodParameters(method, parameters);
                method.DynamicInvoke(parameters);
            }
        }

        [return: MaybeNull]
        public TResult Invoke<TResult>(object[]? parameters)
        {
            var method = Method;
            if (method != null)
            {
                ValidateMethodReturnType(method, typeof(TResult));
                parameters = ValidateMethodParameters(method, parameters);
                return (TResult)method.DynamicInvoke(parameters);
            }
            return default;
        }

        static object[] ValidateMethodParameters(Delegate method, object[]? parameters)
        {
            var prms = method.Method.GetParameters();
            if (prms.Length == 0)
                return Array.Empty<object>();
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (prms.Length != parameters.Length)
            {
                throw new ArgumentException(
                    $"Parameter count mismatch. Method required {prms.Length} but {parameters.Length} parameters is provided");
            }
            return parameters;
        }

        static void ValidateMethodReturnType(Delegate method, Type expectedReturnType)
        {
            var actual = method.Method.ReturnType;
            if (expectedReturnType.IsAssignableFrom(actual) == false)
                throw new ArgumentException($"Return type mismatch. Method returns {actual}. It can't be converted to {expectedReturnType}.");
        }

    }
}
