using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LightMock.Generator
{
    sealed class CompilationContext
    {
        private readonly HashSet<string> tags;
        private readonly List<SyntaxTree> syntaxTrees;

        public CompilationContext()
        {
            tags = new HashSet<string>();
            syntaxTrees = new List<SyntaxTree>();
        }

        public void AddSyntaxTree(SyntaxTree tree)
        {
            lock (syntaxTrees)
                syntaxTrees.Add(tree);
        }

        public CSharpCompilation Update(CSharpCompilation compilation)
        {
            ImmutableArray<SyntaxTree> trees;
            lock (syntaxTrees)
                trees = syntaxTrees.ToImmutableArray();
            return compilation.AddSyntaxTrees(trees);
        }

        public void AddTag(string tag)
        {
            lock(tags)
                tags.Add(tag);
        }

        public bool IsTagExits(string tag)
        {
            lock (tags)
                return tags.Contains(tag);
        }
    }
}
