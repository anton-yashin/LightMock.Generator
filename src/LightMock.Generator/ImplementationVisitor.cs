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
        private readonly string? implementationName;

        public ImplementationVisitor(SymbolDisplayFormat definitionFormat, string? implementationName)
        {
            this.definitionFormat = definitionFormat;
            this.implementationName = implementationName;
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
                .AppendMethodDeclaration(definitionFormat, symbol);
            result.Append("{");
            if (symbol.ReturnsVoid == false)
            {
                result.Append("return default(")
                    .Append(symbol.ReturnType, SymbolDisplayFormats.WithTypeParams)
                    .Append(");");
            }
            result.Append("}");
            if (implementationName != null && symbol.IsInterfaceRequired())
            {
                result.AppendMethodDeclaration(SymbolDisplayFormats.Interface, symbol, p
                    => p.Kind == SymbolDisplayPartKind.ClassName && p.ToString() == symbol.ContainingType.Name
                    ? new SymbolDisplayPart(SymbolDisplayPartKind.ClassName, symbol, Prefix.ProtectedToPublicInterface + implementationName)
                    : p).Append('{');
                if (symbol.ReturnsVoid == false)
                {
                    result.Append(" return default(")
                        .Append(symbol.ReturnType, SymbolDisplayFormats.WithTypeParams)
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

            if (implementationName != null && symbol.IsInterfaceRequired())
            {
                result.Append(symbol, SymbolDisplayFormats.Interface, Mutator);
                appedGetterAndSetter(result, symbol);

                SymbolDisplayPart Mutator(SymbolDisplayPart part)
                {
                    if (part.Kind == SymbolDisplayPartKind.ClassName && implementationName == part.ToString())
                        return new SymbolDisplayPart(part.Kind, part.Symbol, Prefix.ProtectedToPublicInterface + implementationName);
                    return part;
                }
            }

            return result.ToString();
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
            => symbol.ToDisplayString(SymbolDisplayFormats.Interface);

        protected string? VisitEvent(IEventSymbol symbol, Func<StringBuilder, IEventSymbol, StringBuilder> appendEventAddRemove)
        {
            var ct = symbol.ContainingType;
            var result = new StringBuilder();
            if (ct.Name != nameof(Object) && ct.BaseType == null)
            {
                result.Append(symbol, SymbolDisplayFormats.Interface);
                appendEventAddRemove(result, symbol);
                return result.ToString();
            }
            if (symbol.IsCanBeOverriden())
            {
                result.Append("override ")
                    .Append(symbol, SymbolDisplayFormats.AbstractClass);
                appendEventAddRemove(result, symbol);

                if (implementationName != null && symbol.IsInterfaceRequired())
                {
                    result.Append(symbol, SymbolDisplayFormats.Interface, Mutator);
                    appendEventAddRemove(result, symbol);

                    SymbolDisplayPart Mutator(SymbolDisplayPart part)
                    {
                        if (part.Kind == SymbolDisplayPartKind.ClassName && implementationName == part.ToString())
                            return new SymbolDisplayPart(part.Kind, part.Symbol, Prefix.ProtectedToPublicInterface + implementationName);
                        return part;
                    }
                }
                return result.ToString();
            }
            return null;
        }

    }
}
