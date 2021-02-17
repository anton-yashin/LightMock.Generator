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
