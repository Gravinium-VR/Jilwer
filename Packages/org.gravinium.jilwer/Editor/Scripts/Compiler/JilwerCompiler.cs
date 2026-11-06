using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gravinium.Jilwer.Editor.Compiler.Debug;
using Gravinium.Jilwer.Editor.Compiler.Passes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Gravinium.Jilwer.Editor.Compiler
{
    internal static class JilwerCompiler
    {
        private static readonly IJilwerLowerPass[] Passes =
        {
            new IntrinsicLoweringPass(),
            new ConstructorInvocationLoweringPass(),
            new ConstructorLoweringPass(),
            new RuntimeStatePass(),
        };

        internal static IReadOnlyList<SyntaxTree> Lower(
            IReadOnlyList<SyntaxTree> sourceTrees,
            IEnumerable<MetadataReference> references)
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            
            CSharpCompilation compilation =
                CSharpCompilation.Create(
                    "Jilwer.InMemory",
                    sourceTrees,
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var context = new JilwerCompilationContext();
            
            JilwerCompilerDebug.Begin();
            JilwerCompilerDebug.Capture(0, "Original", compilation.SyntaxTrees);

            int passIndex = 1;
            foreach (IJilwerLowerPass pass in Passes)
            {
                if (pass.CanRun(compilation))
                {
                    compilation = RunPass(compilation, pass, context);
                }

                JilwerCompilerDebug.Capture(passIndex++, pass.GetType().Name, compilation.SyntaxTrees);
            }
            
            JilwerCompilerDebug.Capture(passIndex, "Final", compilation.SyntaxTrees);
            
            totalTime.Stop();
            UnityEngine.Debug.Log($"[Jilwer] Compiler finished in {totalTime.ElapsedMilliseconds} ms.");

            return compilation.SyntaxTrees.ToArray();
        }

        private static CSharpCompilation RunPass(
            CSharpCompilation compilation,
            IJilwerLowerPass pass,
            JilwerCompilationContext context)
        {
            var replacements = new List<(SyntaxTree OldTree, SyntaxTree NewTree)>();

            foreach (SyntaxTree tree in compilation.SyntaxTrees)
            {
                SemanticModel semanticModel = compilation.GetSemanticModel(tree, ignoreAccessibility: true);

                SyntaxNode originalRoot = tree.GetRoot();

                SyntaxNode rewrittenRoot = pass.Rewrite(originalRoot, semanticModel, context);

                if (rewrittenRoot == null)
                    throw new InvalidOperationException($"{pass.GetType().Name} returned a null syntax root.");

                if (ReferenceEquals(originalRoot, rewrittenRoot)) continue;
                if (originalRoot.IsEquivalentTo(rewrittenRoot)) continue;

                SyntaxTree rewrittenTree = tree.WithRootAndOptions(rewrittenRoot, tree.Options);

                replacements.Add((tree, rewrittenTree));
            }

            foreach (var replacement in replacements)
            {
                compilation = compilation.ReplaceSyntaxTree(replacement.OldTree, replacement.NewTree);
            }

            return compilation;
        }
    }
}
