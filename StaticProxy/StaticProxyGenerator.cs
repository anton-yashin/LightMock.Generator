using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace StaticProxy
{
    [Generator]
    public class StaticProxyGenerator : ISourceGenerator
    {
        const string KAttributeName = nameof(MockGeneratedAttribute);
        const string KAttributeFile = KAttributeName + ".cs";

        readonly Lazy<string> attribute = new Lazy<string>(() => Utils.LoadResource(KAttributeFile));

        public StaticProxyGenerator()
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource(KAttributeName, SourceText.From(attribute.Value, Encoding.UTF8));
            if (context.Compilation is CSharpCompilation compilation == false)
                return;
            if (context.SyntaxReceiver is StaticProxySyntaxReceiver receiver == false)
                return;


        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new StaticProxySyntaxReceiver());
        }
    }
}
