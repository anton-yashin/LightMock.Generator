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
using System.Linq;

namespace LightMock.Generator
{
    static class SymbolExtensions
    {
        public static bool IsObsolete(this ISymbol @this)
            => (from i in @this.GetAttributes()
                where i.AttributeClass?.ToDisplayString(SymbolDisplayFormats.Namespace) == "System." + nameof(ObsoleteAttribute)
                select i).Any();

        public static string GetObsoleteOrOverrideChunk(this ISymbol @this)
            => @this.IsObsolete() ? "[Obsolete] override " : "override ";

        public static bool IsCanBeOverriden(this ISymbol @this)
            => @this.IsAbstract || @this.IsVirtual;

        public static (string whereClause, IEnumerable<ITypeSymbol> typeArguments)
            GetWhereClauseAndTypeArguments(this INamedTypeSymbol @this)
        {
            IEnumerable<ITypeSymbol> typeArguments = @this.TypeArguments;
            var whereClause = @this.GetWhereClause();

            for (var tsct = @this.ContainingType; tsct != null; tsct = tsct.ContainingType)
            {
                whereClause = tsct.GetWhereClause() + whereClause;
                typeArguments = tsct.TypeArguments.Concat(typeArguments);
            }

            return (whereClause, typeArguments);
        }

        static string GetWhereClause(this INamedTypeSymbol @this)
        {
            var withTypeParams = @this.ToDisplayString(SymbolDisplayFormats.WithTypeParams);
            var withWhereClause = @this.ToDisplayString(SymbolDisplayFormats.WithWhereClause);
            var whereClause = withWhereClause.Replace(withTypeParams, "");
            return whereClause;
        }

    }
}
