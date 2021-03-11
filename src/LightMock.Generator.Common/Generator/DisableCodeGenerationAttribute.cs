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

namespace LightMock.Generator
{
    /// <summary>
    /// Apply this attribute to your assembly to turn off this source code generator.
    /// </summary>
    /// <remarks>
    /// Be aware you can't use <see cref="IMock{T}.ArrangeSetter(Action{T}, string, int)"/>,
    /// <see cref="IMock{T}.AssertSet(Action{T}, string, int)"/> and <see cref="IMock{T}.AssertSet(Action{T}, Invoked, string, int)"/>
    /// because they use AOT transformations. This attribute useful when you want to create a separate library of mocks.
    /// </remarks>
    [AttributeUsage(validOn: AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DisableCodeGenerationAttribute : Attribute
    {
        public DisableCodeGenerationAttribute() { }
    }
}
