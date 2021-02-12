using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace LightMock.Generator
{
    static class SymbolExtensions
    {
        public static bool IsObsolete(this ISymbol @this)
            => (from i in @this.GetAttributes()
                where i.AttributeClass?.ToDisplayString(SymbolDisplayFormats.Namespace) == "System." + nameof(ObsoleteAttribute)
                select i).Any();
    }
}
