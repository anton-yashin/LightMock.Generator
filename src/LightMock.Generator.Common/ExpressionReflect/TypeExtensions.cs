/*****************************************************************************   
	The MIT License (MIT)

	Copyright (c) 2013 Matthias Gernand

	Permission is hereby granted, free of charge, to any person obtaining a copy of
	this software and associated documentation files (the "Software"), to deal in
	the Software without restriction, including without limitation the rights to
	use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
	the Software, and to permit persons to whom the Software is furnished to do so,
	subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
	FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
	COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
	IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
******************************************************************************    
	https://github.com/mgernand
******************************************************************************/
using System;
using System.Reflection;

namespace ExpressionReflect
{
    static class TypeExtensions
    {
        internal static bool IsFunc(this Type type)
        {
            bool isFunc = false;

            if (type.GetTypeInfo().IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                isFunc = definition == typeof(Func<>) ||
                    definition == typeof(Func<,>) ||
                    definition == typeof(Func<,,>) ||
                    definition == typeof(Func<,,,>) ||
                    definition == typeof(Func<,,,,>);
            }

            return isFunc;
        }

        internal static bool IsAction(this Type type)
        {
            bool isAction = type == typeof(Action);

            if (type.GetTypeInfo().IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();

                isAction = isAction ||
                    definition == typeof(Action<>) ||
                    definition == typeof(Action<,>) ||
                    definition == typeof(Action<,,>) ||
                    definition == typeof(Action<,,,>);
            }

            return isAction;
        }

        internal static bool IsPredicate(this Type type)
        {
            bool isPredicate = false;

            if (type.GetTypeInfo().IsGenericType)
            {
                Type definition = type.GetGenericTypeDefinition();
                isPredicate = definition == typeof(Predicate<>);
            }

            return isPredicate;
        }

        internal static bool IsInstanceOfType(this Type type, object obj)
        {
            return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
        }
    }
}