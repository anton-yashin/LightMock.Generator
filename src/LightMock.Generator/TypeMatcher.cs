using Microsoft.CodeAnalysis;
using System;

namespace LightMock.Generator
{
    sealed class TypeMatcher
    {
        private readonly string fullName;
        private readonly int genericArgumentsCount;

        public TypeMatcher(Type toMatch)
        {
            fullName = toMatch.Namespace + "." + toMatch.Name;
            var qi = fullName.IndexOf('`');
            if (qi >= 0)
                fullName = fullName.Substring(0, qi);
            genericArgumentsCount = toMatch.GetGenericArguments().Length;
        }

        public bool IsMatch(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.Namespace) == fullName
                && genericArgumentsCount == symbol.TypeArguments.Length;
        }
    }
}
