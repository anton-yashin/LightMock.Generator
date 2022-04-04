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
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    sealed class AbstractClassSymbolVisitor : SymbolVisitor<string>
    {
        readonly string className;

        public AbstractClassSymbolVisitor(string className)
        {
            this.className = className;
        }

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || symbol.IsCanBeOverriden() == false)
                return null;

            var result = new StringBuilder();
            bool isInterfaceRequired = symbol.IsInterfaceRequired();

            if (isInterfaceRequired)
                AddInterfaceImplementation(symbol, result);

            result.Append(symbol.GetObsoleteOrOverrideChunk())
                .AppendMethodDeclaration(SymbolDisplayFormats.AbstractClass, symbol)
                .AppendMethodBody(isInterfaceRequired ? VariableNames.ProtectedContext : VariableNames.Context, symbol);
            return result.ToString();
        }

        void AddInterfaceImplementation(IMethodSymbol symbol, StringBuilder result)
        {
            result.AppendMethodDeclaration(SymbolDisplayFormats.Interface, symbol, p => ToP2P(p, symbol))
                .AppendMethodBody(VariableNames.ProtectedContext, symbol);
        }

        SymbolDisplayPart ToP2P(SymbolDisplayPart p, ISymbol symbol)
        {
            return p.Kind == SymbolDisplayPartKind.ClassName && p.ToString() == symbol.ContainingType.Name
                ? new SymbolDisplayPart(p.Kind, p.Symbol, Prefix.ProtectedToPublicInterface + className)
                : p;
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (symbol.IsCanBeOverriden() == false)
                return null;

            bool isInterfaceRequired = symbol.IsInterfaceRequired();

            var result = new StringBuilder();
            if (isInterfaceRequired)
                AddInterfaceImplementation(symbol, result);

            result.Append(symbol.GetObsoleteOrOverrideChunk());
            result.AppendParts(symbol
                .ToDisplayParts(SymbolDisplayFormats.AbstractClass)
                .Where(k => k.Kind != SymbolDisplayPartKind.Keyword || k.ToString() != "internal"));
            result.AppendMockGetterAndSetter(isInterfaceRequired ? VariableNames.ProtectedContext : VariableNames.Context, symbol);
            return result.ToString();
        }

        void AddInterfaceImplementation(IPropertySymbol symbol, StringBuilder result)
        {
            result.Append(symbol, SymbolDisplayFormats.Interface, p => ToP2P(p, symbol))
                .AppendProtectedInterfaceGetterAndSetter(VariableNames.ProtectedContext, symbol);
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            if (symbol.IsCanBeOverriden() == false)
                return null;

            var result = new StringBuilder("override ")
                .AppendParts(symbol
                    .ToDisplayParts(SymbolDisplayFormats.AbstractClass)
                    .Where(k => k.Kind != SymbolDisplayPartKind.Keyword || k.ToString() != "internal"))
                .AppendEventAddRemove(VariableNames.PropertiesContext, symbol, methodName: "Invoke");
            if (symbol.IsInterfaceRequired())
                AddInterfaceImplementation(symbol, result);
            return result.ToString();
        }

        void AddInterfaceImplementation(IEventSymbol symbol, StringBuilder result)
        {
            result.Append(symbol, SymbolDisplayFormats.Interface, p => ToP2P(p, symbol))
                .AppendEventAddRemove(VariableNames.PropertiesContext, symbol, methodName: "Invoke");
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass);
        }
    }
}
