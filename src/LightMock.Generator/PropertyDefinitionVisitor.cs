using Microsoft.CodeAnalysis;
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
            var type = symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.Namespace).Replace(".", "_");

            if (symbol.GetMethod != null)
            {
                result
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(' ')
                    .Append(symbol.Name)
                    .Append('_')
                    .Append(type)
                    .Append(Suffix.Getter)
                    .Append("();");
            }
            if (symbol.SetMethod != null)
            {
                result
                    .Append("void ")
                    .Append(symbol.Name)
                    .Append('_')
                    .Append(type)
                    .Append(Suffix.Setter)
                    .Append("(")
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(" prm);");
            }

            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            var result = new StringBuilder();
            if (symbol.AddMethod != null)
            {
                result
                    .Append("void ")
                    .Append(symbol.Name)
                    .Append(Suffix.Add)
                    .Append("(")
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(" prm);");
            }
            if (symbol.RemoveMethod != null)
            {

                result
                    .Append("void ")
                    .Append(symbol.Name)
                    .Append(Suffix.Remove)
                    .Append("(")
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(" prm);");
            }

            return result.ToString();
        }
    }
}
