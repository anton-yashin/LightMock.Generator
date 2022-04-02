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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace LightMock.Generator
{
    static class SymbolExtensions
    {
        public static bool IsObsolete(this ISymbol @this)
            => (from i in @this.GetAttributes()
                let a = i.AttributeClass
                where  a != null && a.Name == nameof(ObsoleteAttribute) && a.ContainingNamespace.Name == nameof(System)
                select i).Any();

        public static string GetObsoleteOrOverrideChunk(this ISymbol @this)
            => @this.IsObsolete() ? "[Obsolete] override " : "override ";

        public static bool IsCanBeOverriden(this ISymbol @this)
            => @this.IsAbstract || @this.IsVirtual;

        public static bool IsInterfaceRequired(this ISymbol @this)
            => @this.IsCanBeOverriden()
            && (@this.DeclaredAccessibility == Accessibility.Protected
                || @this.DeclaredAccessibility == Accessibility.ProtectedOrInternal);


        public static ImmutableArray<INamedTypeSymbol> GetTypeHierarchy(this INamedTypeSymbol @this)
        {
            var builder = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
            for (var tsct = @this; tsct != null; tsct = tsct.ContainingType)
                builder.Add(tsct);
            return builder.ToImmutable();
        }

        public static ImmutableArray<ITypeSymbol> GetTypeArguments(this ImmutableArray<INamedTypeSymbol> @this)
        {
            var builder = ImmutableArray.CreateBuilder<ITypeSymbol>();
            for (int i = @this.Length - 1; i >= 0; i--)
                builder.AddRange(@this[i].TypeArguments);
            return builder.ToImmutable();
        }

        public static string GetWhereClause(this ImmutableArray<INamedTypeSymbol> @this)
        {
            var sb = new StringBuilder();
            foreach (INamedTypeSymbol item in @this)
            {
                var parts = item.ToDisplayParts(SymbolDisplayFormats.WithWhereClause);
                var pos = parts.IndexOf(p => p.Kind == SymbolDisplayPartKind.Keyword && p.ToString() == "where");
                if (pos > 0)
                {
                    sb.AppendParts(new ImmutableArraySegment<SymbolDisplayPart>(parts, pos)).Append(' ');
                }
            }
            return sb.ToString();
        }

        public static bool IsHaveReservedName(this ISymbol symbol)
        {
            switch (symbol.Name)
            {
                case "abstract":
                case "as":
                case "base":
                case "bool":
                case "break":
                case "byte":
                case "case":
                case "catch":
                case "char":
                case "checked":
                case "class":
                case "const":
                case "continue":
                case "decimal":
                case "default":
                case "delegate":
                case "do":
                case "double":
                case "else":
                case "enum":
                case "event":
                case "explicit":
                case "extern":
                case "false":
                case "finally":
                case "fixed":
                case "float":
                case "for":
                case "foreach":
                case "goto":
                case "if":
                case "implicit":
                case "in":
                case "int":
                case "interface":
                case "internal":
                case "is":
                case "lock":
                case "long":
                case "namespace":
                case "new":
                case "null":
                case "object":
                case "operator":
                case "out":
                case "override":
                case "params":
                case "private":
                case "protected":
                case "public":
                case "readonly":
                case "record":
                case "ref":
                case "return":
                case "sbyte":
                case "sealed":
                case "short":
                case "sizeof":
                case "stackalloc":
                case "static":
                case "string":
                case "struct":
                case "switch":
                case "this":
                case "throw":
                case "true":
                case "try":
                case "typeof":
                case "uint":
                case "ulong":
                case "unchecked":
                case "unsafe":
                case "ushort":
                case "using":
                case "virtual":
                case "void":
                case "volatile":
                case "while":
                    return true;
            }
            return false;
        }
    }
}
