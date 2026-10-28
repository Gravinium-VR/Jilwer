using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEditor;

namespace Gravinium.Jilwer.Editor.Compiler.Debug
{
    internal static class JilwerCompilerDebug
    {
        public static bool Enabled = false;
        
        private static readonly string OutputDirectory = Path.GetFullPath("Library/Jilwer/Debug");
        
        private const string MenuPath = "Tools/Jilwer/Debug/Compiler Debug Enabled";
        private const string PrefKey = "Jilwer.Compiler.Debug.Enabled";
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Enabled = EditorPrefs.GetBool(PrefKey, false);
            Menu.SetChecked(MenuPath, Enabled);
        }
        
        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            bool enabled = !Enabled;
            Enabled = enabled;
            EditorPrefs.SetBool(PrefKey, enabled);
            Menu.SetChecked(MenuPath, enabled);
        }
        
        [MenuItem("Tools/Jilwer/Debug/Open Debug Output")]
        private static void OpenOutput()
        {
            Directory.CreateDirectory(DirectoryPath);
            EditorUtility.RevealInFinder(DirectoryPath);
        }

        public static void Begin()
        {
            if (!Enabled) return;

            Directory.CreateDirectory(OutputDirectory);
            
            foreach (string directory in Directory.GetDirectories(OutputDirectory))
                Directory.Delete(directory, true);
        }

        public static void Capture(int index, string name, IEnumerable<SyntaxTree> trees)
        {
            if (!Enabled) return;

            string directory = Path.Combine(OutputDirectory, $"{index:D2}-{name}");
            Directory.CreateDirectory(directory);

            int fileIndex = 0;
            foreach (SyntaxTree tree in trees)
            {
                string fileName = GetFileName(tree, fileIndex++);
                File.WriteAllText(Path.Combine(directory, fileName), tree.GetRoot().NormalizeWhitespace().ToFullString());
            }
        }

        private static string GetFileName(SyntaxTree tree, int index)
        {
            var root = tree.GetRoot();
            var type = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            string name = type?.Identifier.ValueText ?? "Module";

            return $"{index:D3}-{name}.cs";
        }

        public static string DirectoryPath => OutputDirectory;
    }
}