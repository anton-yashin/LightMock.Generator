using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightMock.Generator
{
    sealed class AssertImplementationVisitor : SymbolVisitor<string>
    {
        private readonly SymbolDisplayFormat definitionFormat;

        public AssertImplementationVisitor(SymbolDisplayFormat definitionFormat)
        {
            this.definitionFormat = definitionFormat;
        }

        static bool IsCanBeOverriden(ISymbol symbol)
            => symbol.IsAbstract || symbol.IsVirtual;

        static string GetOverrideChunkFor(ISymbol symbol)
            => (symbol.ContainingType.Name == nameof(Object) || symbol.ContainingType.BaseType != null) ? "override " : "";

        public override string? VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind != MethodKind.Ordinary || IsCanBeOverriden(symbol) == false)
                return null;
            var result = new StringBuilder()
                .Append(GetOverrideChunkFor(symbol))
                .AppendMethodDeclaration(symbol.ToDisplayString(definitionFormat), symbol);
            result.Append("{");
            if (symbol.ReturnsVoid == false)
            {
                result.Append("return default(")
                    .Append(symbol.ReturnType.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(");");
            }
            result.Append("}");
            return result.ToString();
        }

        public override string? VisitProperty(IPropertySymbol symbol)
        {
            if (IsCanBeOverriden(symbol) == false)
                return null;

            var result = new StringBuilder(GetOverrideChunkFor(symbol))
                .Append(symbol.ToDisplayString(definitionFormat))
                .Append("{");

            if (symbol.GetMethod != null)
            {
                result.Append("get { ")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .Append(symbol.Name)
                    .Append(Suffix.Getter)
                    .Append("(), ")
                    .Append(VariableNames.Invoked)
                    .Append("); return default(")
                    .Append(symbol.Type.ToDisplayString(SymbolDisplayFormats.Interface))
                    .Append(");}");
            }
            if (symbol.SetMethod != null)
            {
                result.Append("set {")
                    .Append(VariableNames.Context)
                    .Append(".Assert(f => f.")
                    .Append(symbol.Name)
                    .Append(Suffix.Setter)
                    .Append("(value), ")
                    .Append(VariableNames.Invoked)
                    .Append("); }");
            }

            result.Append("}");

            return result.ToString();
        }

        public override string? VisitEvent(IEventSymbol symbol)
        {
            const string methodName = "Assert";
            var ct = symbol.ContainingType;
            var result = new StringBuilder();
            if (ct.Name != nameof(Object) && ct.BaseType == null)
            {
                result.Append(symbol.ToDisplayString(SymbolDisplayFormats.Interface))
                    .AppendEventAddRemove(VariableNames.Context, symbol, methodName);
                return result.ToString();
            }
            if (IsCanBeOverriden(symbol))
            {
                result.Append("override ")
                    .Append(symbol.ToDisplayString(SymbolDisplayFormats.AbstractClass))
                    .AppendEventAddRemove(VariableNames.Context, symbol, methodName);
                return result.ToString();
            }
            return null;
        }

        public override string? VisitNamedType(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormats.Interface);
        }
    }
}
