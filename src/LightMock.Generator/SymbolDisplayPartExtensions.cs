using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LightMock.Generator
{
    static class SymbolDisplayPartExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInternal(this SymbolDisplayPart sdp)
            => sdp.Kind == SymbolDisplayPartKind.Keyword && sdp.ToString() == "internal";

        public static IEnumerable<SymbolDisplayPart> FilterInternalKeywordFromNonFriendDeclarations(
            this IEnumerable<SymbolDisplayPart> parts,
            ISymbol symbol, Compilation compilation)
        {
            return symbol.ContainingAssembly.GivesAccessTo(compilation.Assembly)
                ? parts
                : parts.Where(k => k.IsInternal() == false);
        }
    }
}
