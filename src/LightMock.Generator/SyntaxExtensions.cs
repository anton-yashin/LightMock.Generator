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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace LightMock.Generator
{
    static class SyntaxExtensions
    {
        /// <summary>
        /// Get argument from <see cref="ArgumentListSyntax"/> by name or index.
        /// </summary>
        /// <param name="this">List of arguments</param>
        /// <param name="argumentName">Name of argument to be search</param>
        /// <param name="argumentIndex">Index of argument</param>
        /// <returns>Argument or null if there no argument with specified <paramref name="argumentName"/> or <paramref name="argumentIndex"/> out of range</returns>
        public static ArgumentSyntax? GetArgument(this ArgumentListSyntax @this, string argumentName, int argumentIndex)
        {
            var result = @this.Arguments.FirstOrDefault(i => i.NameColon?.Name.ToString() == argumentName);
            return result ?? ((@this.Arguments.Count > argumentIndex) ? @this.Arguments[argumentIndex] : null);
        }
    }
}
