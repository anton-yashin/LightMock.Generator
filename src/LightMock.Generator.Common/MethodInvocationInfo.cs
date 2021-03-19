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
using LightMock.Generator;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LightMock
{
    sealed class MethodInvocationInfo : IInvocationInfo
    {
        public MethodInvocationInfo(MethodInfo method, object[] arguments)
        {
            this.Method = method;
            this.Arguments = arguments;
        }

        public MethodInfo Method { get; }

        public object[] Arguments { get; }

        public string AsString()
        {
            var result = new StringBuilder();
            result.Append(Method.ReturnType)
                .Append(' ');
            AppendDeclaringType(result);
            AppendParameters(result);
            return result.ToString();
        }

        void AppendParameters(StringBuilder here)
        {
            var ona = Method.GetCustomAttribute<OriginalNameAttribute>();
            if (ona != null)
                here.AppendFormat(ona.OriginalName, Arguments.Select(i => ArgumentValueToString(i))
                    .Concat(Enumerable.Repeat<object?>(null, ona.ParametersCount))
                    .Take(ona.ParametersCount).ToArray());
            else
                DefaultAppendParameters(here);
        }

        void DefaultAppendParameters(StringBuilder here)
        {
            here.Append('.')
                .Append(Method.Name)
                .Append('(');
            var parameters = Method.GetParameters();
            var last = Math.Min(Arguments.Length, parameters.Length);
            for (int i = 0; i < last; i++)
            {
                var p = parameters[i];
                var a = Arguments[i];
                if (i > 0)
                    here.Append(',');
                here.Append(p.ParameterType)
                    .Append(' ')
                    .Append(p.Name)
                    .Append(" = ")
                    .Append(ArgumentValueToString(a));
            }
            here.Append(')');
        }

        void AppendDeclaringType(StringBuilder stringBuilder)
        {
            var mdt = Method.DeclaringType;
            var ona = mdt.GetCustomAttribute<OriginalNameAttribute>();
            if (ona != null)
            {
                var ta = mdt.IsGenericType ? mdt.GetGenericArguments() : Array.Empty<Type>();
                stringBuilder.AppendFormat(ona.OriginalName, ta);
            }
            else
            {
                stringBuilder.Append(mdt);
            }
        }

        static string ArgumentValueToString(object value)
        {
            if (value == null)
                return "null";
            switch (value)
            {
                case float f:
                    return f.ToString("G9");
                case double d:
                    return d.ToString("G17");
                case Enum e:
                    return e.GetType().ToString() + '.' + e.ToString("F");
                case string s:
                    return '"' + s + '"';
            }
            return value.ToString();
        }

        public void Invoke(CallbackInvocation callback) => callback.Invoke(Arguments);

        [return: MaybeNull]
        public TResult Invoke<TResult>(CallbackInvocation callback, [AllowNull] TResult defaultValue)
            => callback.Invoke(Arguments, defaultValue);

        public bool IsMethod => Method.IsSpecialName == false;
    }
}
