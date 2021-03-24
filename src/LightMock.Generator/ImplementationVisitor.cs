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
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    abstract class ImplementationVisitor : SymbolVisitor<string>
    {
        protected readonly SymbolDisplayFormat definitionFormat;
        private readonly string? p2pInterfaceName;

        public ImplementationVisitor(SymbolDisplayFormat definitionFormat, string? p2pInterfaceName)
        {
            this.definitionFormat = definitionFormat;
            this.p2pInterfaceName = p2pInterfaceName;
        }

        protected static string GetObsoleteAndOrOverrideChunkFor(ISymbol symbol)
            => (symbol.ContainingType.Name == nameof(Object) || symbol.ContainingType.BaseType != null)
            ? symbol.GetObsoleteOrOverrideChunk() : "";

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || symbol.IsCanBeOverriden() == false)
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
            if (p2pInterfaceName != null && symbol.IsInterfaceRequired())
            {
                var parts = symbol.ToDisplayParts(SymbolDisplayFormats.Interface).Select(p
                    => p.Kind == SymbolDisplayPartKind.ClassName && p.ToString().Contains(symbol.ContainingType.Name)
                    ? new SymbolDisplayPart(SymbolDisplayPartKind.ClassName, symbol, p2pInterfaceName.Split('<').First())
                    : p).ToImmutableArray();
                result.AppendMethodDeclaration(parts.ToDisplayString(), symbol).Append('{');
                if (symbol.ReturnsVoid == false)
                {
                    result.Append(" return default(")
                        .Append(symbol.ReturnType.ToDisplayString(SymbolDisplayFormats.WithTypeParams))
                        .Append("); ");
                }
                result.Append('}');
            }
            return result.ToString();
        }

        protected string? VisitProperty(IPropertySymbol symbol, Func<StringBuilder, IPropertySymbol, StringBuilder> appedGetterAndSetter)
        {
            if (symbol.IsCanBeOverriden() == false)
                return null;

            var result = new StringBuilder(GetObsoleteAndOrOverrideChunkFor(symbol))
                .Append(symbol, definitionFormat);
            appedGetterAndSetter(result, symbol);

            if (p2pInterfaceName != null && symbol.IsInterfaceRequired())
            {
                var name = p2pInterfaceName.Split('<').First();
                if (p2pInterfaceName == "IP2P_AAbstractClassWithMultipleNamespaces")
                { }
                result.Append(symbol, SymbolDisplayFormats.Interface, Mutator);
                appedGetterAndSetter(result, symbol);

                SymbolDisplayPart Mutator(SymbolDisplayPart part)
                {
                    var s = part.ToString();
                    var xs = symbol;
                    switch (part.Kind)
                    {
                        case SymbolDisplayPartKind.ClassName when name.Contains(s):
                            return new SymbolDisplayPart(part.Kind, part.Symbol, name);
                    }
                    return part;
                }
            }

            return result.ToString();


        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
            => symbol.ToDisplayString(SymbolDisplayFormats.Interface);

        protected static string? VisitEvent(IEventSymbol symbol, Func<StringBuilder, IEventSymbol, StringBuilder> appendEventAddRemove)
        {
            var ct = symbol.ContainingType;
            var result = new StringBuilder();
            if (ct.Name != nameof(Object) && ct.BaseType == null)
            {
                result.Append(symbol.ToDisplayString(SymbolDisplayFormats.Interface));
                appendEventAddRemove(result, symbol);
                return result.ToString();
            }
            if (symbol.IsCanBeOverriden())
            {
                result.Append("override ")
                    .Append(symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass));
                appendEventAddRemove(result, symbol);
                return result.ToString();
            }
            return null;
        }

    }
}
