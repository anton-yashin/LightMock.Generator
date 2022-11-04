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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using LightMock.Generator;

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

		public void Invoke(CallbackInvocation callback, IDictionary<string, object>? refValues)
		{
			callback.Invoke(Arguments);
			SetRefParameters(refValues);
		}

		[return: MaybeNull]
		public TResult Invoke<TResult>(CallbackInvocation callback, [AllowNull] TResult defaultValue, IDictionary<string, object>? refValues)
		{
			var result = callback.Invoke(Arguments, defaultValue);
            SetRefParameters(refValues);
			return result;
        }

        void SetRefParameters(IDictionary<string, object>? refValues)
		{
            if (refValues == null)
                return;
            var parameters = Method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.ParameterType.IsByRef)
                {
                    refValues[parameter.Name] = Arguments[i];
                }
            }
        }

        public void AppendInvocationInfo(StringBuilder here)
        {
			here.Append(Method.ReturnType).Append(' ');
			AppendDeclaringType(here);
			AppendParameters(here);
		}

		public bool IsMethod => Method.IsSpecialName == false;

		private void AppendParameters(StringBuilder here)
		{
			var customAttribute = Method.GetCustomAttribute<OriginalNameAttribute>();
			if (customAttribute != null)
			{
				here.AppendFormat(customAttribute.OriginalNameFormat,
					args: Arguments.Select(i => ArgumentValueToString(i))
					.Concat(Enumerable.Repeat<object?>(null, customAttribute.ParametersCount))
					.Take(customAttribute.ParametersCount)
					.ToArray());
			}
			else
			{
				DefaultAppendParameters(here);
			}
		}

		private void DefaultAppendParameters(StringBuilder here)
		{
			here.Append('.').Append(Method.Name).Append('(');
            var parameters = Method.GetParameters();
			int num = Math.Min(Arguments.Length, parameters.Length);
			for (int i = 0; i < num; i++)
			{
				var parameterInfo = parameters[i];
				var value = Arguments[i];
				if (i > 0)
					here.Append(',');
				here.Append(parameterInfo.ParameterType).Append(' ').Append(parameterInfo.Name)
					.Append(" = ")
					.Append(ArgumentValueToString(value));
			}
			here.Append(')');
		}

		private void AppendDeclaringType(StringBuilder stringBuilder)
		{
			var declaringType = Method.DeclaringType;
			var customAttribute = declaringType.GetCustomAttribute<OriginalNameAttribute>();
			if (customAttribute != null)
			{
				stringBuilder.AppendFormat(customAttribute.OriginalNameFormat,
					args: declaringType.IsGenericType ? declaringType.GetGenericArguments() : Array.Empty<Type>());
			}
			else
			{
				stringBuilder.Append(declaringType);
			}
		}

		private static string ArgumentValueToString(object value)
		{
			if (value == null)
				return "null";
			switch (value)
            {
				case string s:
					return "\"" + s + "\"";
				case float f:
					return f.ToString("G9");
				case double d:
					return d.ToString("G17");
				case Enum @enum:
					return @enum.GetType().ToString() + "." + @enum.ToString("F");
				case Delegate d:
					return d.Method.ToString();
            }
			return value.ToString() ?? value.GetType().ToString();
		}
	}
}
