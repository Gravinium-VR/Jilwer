using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Gravinium.Jilwer.Editor.Compiler.Debug
{
    internal static class JilwerCompilerPreview
    {
        private const string PreviewDirectory = "Library/Jilwer/CompilerPreview";

        [MenuItem("Tools/Jilwer/Debug/Generate Compiler Preview", priority = 1)]
        public static void GenerateLoaded()
        {
            Generate(JilwerCompiler.CompileLoadedScenes);
        }
        
        [MenuItem("Tools/Jilwer/Debug/Generate Full Compiler Preview (slow)", priority = 2)]
        public static void GenerateAll()
        {
            Generate(JilwerCompiler.CompileAll);
        }

        private static void Generate(Func<IReadOnlyList<JilwerCompilationResult>> compileFunc)
        {
            if (Directory.Exists(PreviewDirectory)) Directory.Delete(PreviewDirectory, true);

            Directory.CreateDirectory(PreviewDirectory);

            var results = compileFunc();

            int generatedCount = 0;

            foreach (JilwerCompilationResult result in results)
            {
                foreach (JilwerGeneratedSource source in result.Sources)
                {
                    WritePreview(source);
                    generatedCount++;
                }
            }

            UnityEngine.Debug.Log($"[Jilwer] Generated {generatedCount} transformed file(s).");
        }

        private static void WritePreview(JilwerGeneratedSource source)
        {
            string assemblyDirectory = Path.Combine(PreviewDirectory, Sanitize(source.AssemblyName));
            string projectRoot = Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, ".."));
            string sourcePath = Path.GetFullPath(source.SourcePath);
            string relativePath = Path.GetRelativePath(projectRoot, sourcePath);
            string outputPath = Path.Combine(assemblyDirectory, relativePath);
            string outputDirectory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrEmpty(outputDirectory)) Directory.CreateDirectory(outputDirectory);
            
            File.WriteAllText(outputPath, source.GeneratedSource);
            
            UnityEngine.Debug.Log($"[Jilwer] {source.SourcePath} -> {outputPath}");
        }

        private static string Sanitize(string value)
        {
            return Path.GetInvalidFileNameChars().Aggregate(value, (current, c) => current.Replace(c, '_'));
        }
    }
}