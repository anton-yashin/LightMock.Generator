using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    sealed class RefReturnVisitor : SymbolVisitor<string>
    {
        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.ReturnsByRef || symbol.ReturnsByRefReadonly)
            {
                var result = symbol.ToDisplayString(SymbolDisplayFormats.KRefReturnInterfaceDeclaration) + ";";
                return result;
            }
            return "";
        }

    }
}
