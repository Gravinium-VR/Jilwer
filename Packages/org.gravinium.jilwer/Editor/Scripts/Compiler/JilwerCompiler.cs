using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gravinium.Jilwer.Editor.Compiler.Passes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using UnityEditor.Compilation;

namespace Gravinium.Jilwer.Editor.Compiler
{
    internal static class JilwerCompiler
    {
        private static readonly IJilwerLowerPass[] Passes =
        {
            new IntrinsicLoweringPass(),
            new RuntimeStatePass(),
        };

        public static IReadOnlyList<JilwerCompilationResult> CompileAll()
        {
            List<JilwerCompilationResult> results = new();

            Assembly[] assemblies = CompilationPipeline.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if ((assembly.flags & AssemblyFlags.EditorAssembly) != 0) continue;

                JilwerCompilationResult result = CompileAssembly(assembly);
                
                if (result.Sources.Count > 0) results.Add(result);
            }

            return results;
        }

        private static JilwerCompilationResult CompileAssembly(Assembly assembly)
        {
            string[] sourceFiles = assembly.sourceFiles.Where(File.Exists).ToArray();

            if (sourceFiles.Length == 0)
            {
                return new JilwerCompilationResult(assembly.name, Array.Empty<JilwerGeneratedSource>(),
                    new JilwerCompilationContext());
            }

            CSharpParseOptions parseOptions =
                new CSharpParseOptions(LanguageVersion.CSharp9, preprocessorSymbols: assembly.defines);

            SyntaxTree[] originalTrees = sourceFiles
                .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), parseOptions, path)).ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create($"Jilwer.{assembly.name}", originalTrees,
                GetReferences(assembly), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            JilwerCompilationContext context = new JilwerCompilationContext();

            foreach (IJilwerLowerPass pass in Passes)
            {
                if (!pass.CanRun(compilation)) continue;

                compilation = RunPass(compilation, pass, context);
            }

            List<JilwerGeneratedSource> generated = BuildResults(assembly.name, originalTrees, compilation);

            return new JilwerCompilationResult(assembly.name, generated, context);
        }

        private static CSharpCompilation RunPass(CSharpCompilation compilation, IJilwerLowerPass pass,
            JilwerCompilationContext context)
        {
            List<(SyntaxTree OldTree, SyntaxTree NewTree)> replacements = new();

            foreach (SyntaxTree tree in compilation.SyntaxTrees)
            {
                SemanticModel semanticModel = compilation.GetSemanticModel(tree, ignoreAccessibility: true);

                SyntaxNode originalRoot = tree.GetRoot();

                SyntaxNode rewrittenRoot = pass.Rewrite(originalRoot, semanticModel, context);

                if (ReferenceEquals(originalRoot, rewrittenRoot)) continue;
                if (originalRoot.IsEquivalentTo(rewrittenRoot)) continue;

                SyntaxTree rewrittenTree = tree.WithRootAndOptions(rewrittenRoot, tree.Options);
                
                replacements.Add((tree, rewrittenTree));
            }

            foreach ((SyntaxTree oldTree, SyntaxTree newTree) in replacements)
            {
                compilation = compilation.ReplaceSyntaxTree(oldTree, newTree);
            }

            return compilation;
        }

        private static List<JilwerGeneratedSource> BuildResults(string assemblyName,
            IEnumerable<SyntaxTree> originalTrees, CSharpCompilation finalCompilation)
        {
            Dictionary<string, SyntaxTree> finalTrees =
                finalCompilation.SyntaxTrees.ToDictionary(tree => tree.FilePath, StringComparer.OrdinalIgnoreCase);

            List<JilwerGeneratedSource> results = new();

            foreach (SyntaxTree originalTree in originalTrees)
            {
                if (!finalTrees.TryGetValue(originalTree.FilePath, out SyntaxTree finalTree)) continue;

                string original = originalTree.GetRoot().ToFullString();
                string generated = finalTree.GetRoot().ToFullString();

                if (original == generated) continue;
                
                results.Add(new JilwerGeneratedSource(assemblyName, originalTree.FilePath, original, generated));
            }
            
            return results;
        }

        private static List<MetadataReference> GetReferences(Assembly assembly)
        {
            HashSet<string> paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string path in assembly.compiledAssemblyReferences)
            {
                if (File.Exists(path)) paths.Add(path);
            }

            foreach (Assembly referencedAssembly in assembly.assemblyReferences)
            {
                string path = referencedAssembly.outputPath;

                if (!string.IsNullOrEmpty(path) && File.Exists(path)) paths.Add(path);
            }

            AddAssemblyReference(paths, typeof(object).Assembly.Location);
            AddAssemblyReference(paths, typeof(UnityEngine.Object).Assembly.Location);

            return paths.Select(path => MetadataReference.CreateFromFile(path)).Cast<MetadataReference>().ToList();
        }

        private static void AddAssemblyReference(HashSet<string> paths, string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) paths.Add(path);
        }
    }
}