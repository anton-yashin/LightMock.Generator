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
using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace LightMock.Generator
{
    sealed class InterfaceSymbolVisitor : SymbolVisitor<string>
    {
        public InterfaceSymbolVisitor()
        { }

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary)
                return null;

            var result = new StringBuilder()
                .AppendMethodDeclaration(symbol.ToDisplayString(SymbolDisplayFormats.Interface), symbol)
                .AppendMethodBody(VariableNames.Context, symbol,
                symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.WithTypeParams));
            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            var result = new StringBuilder()
                .Append(symbol, SymbolDisplayFormats.Interface)
                .AppendMockGetterAndSetter(VariableNames.Context,
                symbol, symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.WithTypeParams));
            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            var result = new StringBuilder()
                .Append(symbol, SymbolDisplayFormats.Interface)
                .AppendEventAddRemove(VariableNames.PropertiesContext, symbol, methodName: "Invoke");
            return result.ToString();
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.Interface);
        }
    }
}
