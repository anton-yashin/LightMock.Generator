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
    sealed class ArrangeOnImplementationVisitor : SymbolVisitor<string>
    {
        private readonly SymbolDisplayFormat definitionFormat;
        private readonly string propertyToFuncInterfaceName;

        public ArrangeOnImplementationVisitor(SymbolDisplayFormat definitionFormat, string propertyToFuncInterfaceName)
        {
            this.definitionFormat = definitionFormat;
            this.propertyToFuncInterfaceName = propertyToFuncInterfaceName;
        }

        static bool IsCanBeOverriden(ISymbol symbol)
            => symbol.IsAbstract || symbol.IsVirtual;

        static string GetObsoleteAndOrOverrideChunkFor(ISymbol symbol)
            => (symbol.ContainingType.Name == nameof(Object) || symbol.ContainingType.BaseType != null) 
            ? (symbol.IsObsolete() ? "[Obsolete] override " : "override ")
            : "";

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || IsCanBeOverriden(symbol) == false)
                return null;
            var result = new StringBuilder()
                .Append(GetObsoleteAndOrOverrideChunkFor(symbol))
                .AppendMethodDeclaration(symbol.ToDisplayString(definitionFormat), symbol);
            result.Append("{");
            if (symbol.ReturnsVoid == false)
            {
                result.Append("return default(")
                    .Append(symbol.ReturnType.ToDisplayString(SymbolDisplayFormats.WithTypeParams))
                    .Append(");");
            }
            result.Append("}");
            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (IsCanBeOverriden(symbol) == false)
                return null;

            var result = new StringBuilder(GetObsoleteAndOrOverrideChunkFor(symbol))
                .Append(symbol.ToDisplayString(definitionFormat))
                .AppendArrangeOnGetterAndSetter(symbol, propertyToFuncInterfaceName);

            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            var ct = symbol.ContainingType;
            var result = new StringBuilder();
            if (ct.Name != nameof(Object) && ct.BaseType == null)
            {
                result.Append(symbol.ToDisplayString(SymbolDisplayFormats.Interface))
                    .AppendDummyEventAddRemove(symbol);
                return result.ToString();
            }
            if (IsCanBeOverriden(symbol))
            {
                result.Append("override ")
                    .Append(symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass))
                    .AppendDummyEventAddRemove(symbol);
                return result.ToString();
            }
            return null;
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.Interface);
        }
    }
}
