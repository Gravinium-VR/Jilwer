
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Gravinium.Jilwer.Editor.Compiler.Integration
{
    [InitializeOnLoad]
    internal static class JilwerUdonCompileHook
    {
        private const string HarmonyId = "org.gravinium.jilwer.compiler";

        private static Harmony _harmony;
        private static IReadOnlyList<MetadataReference> _cachedReferences = Array.Empty<MetadataReference>();

        static JilwerUdonCompileHook()
        {
            RefreshReferences();
            Install();
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }

        private static void Install()
        {
            if (_harmony != null) return;

            try
            {
                Type contextType = AccessTools.TypeByName("UdonSharp.Compiler.CompilationContext");

                if (contextType == null)
                    throw new MissingMemberException("UdonSharp CompilationContext");

                MethodInfo target = contextType
                    .GetMethods(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic)
                    .Single(m =>
                        m.Name == "LoadSyntaxTreesAndCreateModules" &&
                        m.GetParameters().Length == 1 &&
                        m.ReturnType.IsArray);

                Type moduleType = target.ReturnType.GetElementType();

                FieldInfo treeField = moduleType?.GetField(
                    "tree",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

                if (treeField == null || !typeof(SyntaxTree).IsAssignableFrom(treeField.FieldType))
                {
                    throw new MissingFieldException("UdonSharp ModuleBinding.tree");
                }

                _harmony = new Harmony(HarmonyId);

                _harmony.Patch(
                    target,
                    postfix: new HarmonyMethod(typeof(JilwerUdonCompileHook), nameof(AfterModulesLoaded)));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[Jilwer] Failed to install compiler integration: " + ex);
            }
        }
        
        private static void OnCompilationFinished(object context)
        {
            // The comp can happen during assembly comp so this refreshes everything on the main thread.
            EditorApplication.delayCall += RefreshReferences;
        }

        private static void RefreshReferences()
        {
            try
            {
                IReadOnlyList<MetadataReference> references = BuildReferences();

                System.Threading.Volatile.Write(ref _cachedReferences, references);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[Jilwer] Failed to refresh metadata references: " + ex);
            }
        }

        private static void AfterModulesLoaded(object __result)
        {
            UnityEngine.Debug.Log("[Jilwer] UdonSharp compilation intercepted, running transformations...");

            if (!(__result is Array modules) || modules.Length == 0)
            {
                UnityEngine.Debug.LogWarning("[Jilwer] UdonSharp returned no modules.");
                return;
            }

            FieldInfo treeField = modules.GetValue(0)
                .GetType()
                .GetField(
                    "tree",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            if (treeField == null) throw new MissingFieldException("ModuleBinding.tree");

            var originals = new List<SyntaxTree>(modules.Length);

            foreach (object module in modules)
            {
                if (!(treeField.GetValue(module) is SyntaxTree tree))
                {
                    throw new InvalidOperationException("UdonSharp returned a module without a syntax tree.");
                }

                originals.Add(tree);
            }

            IReadOnlyList<MetadataReference> references = System.Threading.Volatile.Read(ref _cachedReferences);

            if (references.Count == 0)
            {
                throw new InvalidOperationException("[Jilwer] Metadata references are not initialized.");
            }

            IReadOnlyList<SyntaxTree> lowered = JilwerCompiler.Lower(originals, references);

            if (lowered.Count != modules.Length)
                throw new InvalidOperationException("Jilwer changed the UdonSharp module count.");

            for (int i = 0; i < modules.Length; i++)
            {
                SyntaxTree original = originals[i];
                SyntaxTree rewritten = lowered[i];

                if (ReferenceEquals(original, rewritten)) continue;

                if (original.GetRoot().IsEquivalentTo(rewritten.GetRoot())) continue;

                object module = modules.GetValue(i);

                treeField.SetValue(module, rewritten);

                if (module.GetType().IsValueType) modules.SetValue(module, i);
            }
        }

        private static IReadOnlyList<MetadataReference> BuildReferences()
        {
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var assembly in CompilationPipeline.GetAssemblies())
            {
                foreach (string reference in assembly.compiledAssemblyReferences)
                {
                    AddReference(paths, reference);
                }

                foreach (var dependency in assembly.assemblyReferences)
                {
                    AddReference(paths, dependency.outputPath);
                }

                AddReference(paths, assembly.outputPath);
            }

            foreach (System.Reflection.Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (loaded.IsDynamic) continue;

                try
                {
                    AddReference(paths, loaded.Location);
                }
                catch (NotSupportedException)
                {
                    // runtime assembly doesn't have a file loc
                }
            }

            return paths
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray();
        }

        private static void AddReference(HashSet<string> paths, string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) paths.Add(Path.GetFullPath(path));
        }
    }
}
