using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    public static class IntrinsicProcessor
    {
        private const string PreviewDirectory = "Library/Jilwer/IntrinsicPreview";

        [MenuItem("Tools/Jilwer/Debug/Generate Intrinsic Preview")]
        public static void GeneratePreview()
        {
            if (Directory.Exists(PreviewDirectory)) Directory.Delete(PreviewDirectory, true);

            Directory.CreateDirectory(PreviewDirectory);

            Assembly[] assemblies = CompilationPipeline.GetAssemblies();

            int transformedFiles = 0;

            foreach (Assembly assembly in assemblies)
            {
                if ((assembly.flags & AssemblyFlags.EditorAssembly) != 0) continue;
                transformedFiles += ProcessAssembly(assembly);
            }
            
            Debug.Log($"[Jilwer] Intrinsic preview complete. " + $"{transformedFiles} file(s) transformed.");
        }

        private static int ProcessAssembly(Assembly assembly)
        {
            string[] sourceFiles = assembly.sourceFiles.Where(File.Exists).ToArray();

            if (sourceFiles.Length == 0) return 0;

            CSharpParseOptions parseOptions =
                new CSharpParseOptions(LanguageVersion.CSharp9, preprocessorSymbols: assembly.defines);

            SyntaxTree[] syntaxTree = sourceFiles.Select(path =>
                CSharpSyntaxTree.ParseText(File.ReadAllText(path), parseOptions, path)).ToArray();

            List<MetadataReference> references = GetReferences(assembly);

            CSharpCompilation compilation = CSharpCompilation.Create($"Jilwer.Intrinsic.{assembly.name}", syntaxTree,
                references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            int transformedCount = 0;

            foreach (SyntaxTree tree in syntaxTree)
            {
                SemanticModel semanticModel = compilation.GetSemanticModel(tree, ignoreAccessibility: true);

                SyntaxNode originalRoot = tree.GetRoot();

                IntrinsicRewriter rewriter = new IntrinsicRewriter(semanticModel);

                SyntaxNode rewrittenRoot = rewriter.Visit(originalRoot);

                string original = originalRoot.ToFullString();
                string rewritten = rewrittenRoot.ToFullString();
                
                if (original == rewritten) continue;

                WritePreview(assembly.name, tree.FilePath, rewritten);
                transformedCount++;
            }

            return transformedCount;
        }

        private static List<MetadataReference> GetReferences(Assembly assembly)
        {
            HashSet<string> paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            foreach (string path in assembly.compiledAssemblyReferences)
                if (File.Exists(path))
                    paths.Add(path);
            
            foreach (Assembly referencedAssembly in assembly.assemblyReferences)
                if (!string.IsNullOrEmpty(referencedAssembly.outputPath) && File.Exists(referencedAssembly.outputPath))
                    paths.Add(referencedAssembly.outputPath);

            AddAssemblyReference(paths, typeof(object).Assembly.Location);
            AddAssemblyReference(paths, typeof(UnityEngine.Object).Assembly.Location);

            return paths.Select(path => MetadataReference.CreateFromFile(path)).ToList<MetadataReference>();
        }

        private static void AddAssemblyReference(HashSet<string> paths, string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) paths.Add(path);
        }

        private static void WritePreview(string assemblyName, string sourcePath, string contents)
        {
            string fileName = Path.GetFileName(sourcePath);
            string outputDirectory = Path.Combine(PreviewDirectory, SanitizeFileName(assemblyName));

            Directory.CreateDirectory(outputDirectory);

            string outputPath = Path.Combine(outputDirectory, fileName);

            File.WriteAllText(outputPath, contents);
            
            Debug.Log($"[Jilwer] Intrinsic transform: " +
                      $"{sourcePath} -> {outputPath}");
        }

        private static string SanitizeFileName(string value)
        {
            return Path.GetInvalidFileNameChars().Aggregate(value, (current, c) => current.Replace(c, '_'));
        }
    }
}