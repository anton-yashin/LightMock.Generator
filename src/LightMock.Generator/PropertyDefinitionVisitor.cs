﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    sealed class PropertyDefinitionVisitor : SymbolVisitor<string>
    {
        public override string? VisitProperty(IPropertySymbol symbol)
        {
            var result = new StringBuilder();

            if (symbol.GetMethod != null)
            {
                result
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(" ")
                    .Append(symbol.Name)
                    .Append(Suffix.Getter)
                    .Append("();");
            }
            if (symbol.SetMethod != null)
            {
                result
                    .Append("void ")
                    .Append(symbol.Name)
                    .Append(Suffix.Setter)
                    .Append("(")
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(" prm);");
            }

            return result.ToString();
        }
    }
}