using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UdonSharp;
using UnityEditor;

namespace Gravinium.Jilwer.Editor.Compiler.Integration
{
    [InitializeOnLoad]
    internal static class JilwerUdonCompileHook
    {
        private const string HarmonyId = "org.gravinium.jilwer.compiler";

        private static bool _insideJilwerCompile;

        static JilwerUdonCompileHook()
        {
            MethodInfo target = AccessTools.Method(
                typeof(UdonSharpProgramAsset),
                nameof(UdonSharpProgramAsset.CompileAllCsPrograms),
                new[] {typeof(bool), typeof(bool)});

            if (target == null)
            {
                UnityEngine.Debug.LogError($"[Jilwer] Could not find UdonSharpProgramAsset.CompileAllCsPrograms.");
                return;
            }

            Harmony harmony = new Harmony(HarmonyId);

            harmony.Patch(target,
                prefix: new HarmonyMethod(typeof(JilwerUdonCompileHook), nameof(BeforeUdonSharpCompile)));
        }

        private static bool BeforeUdonSharpCompile(bool forceCompile, bool editorBuild)
        {
            if (_insideJilwerCompile) return true;

            var results = JilwerCompiler.CompileLoadedScenes();

            var generatedSources = results.SelectMany(result => result.Sources).ToArray();

            if (generatedSources.Length == 0) return true;
            
            UnityEngine.Debug.Log($"[Jilwer] Lowering {generatedSources.Length} source file(s).");

            using JilwerSourceTransaction transaction = new JilwerSourceTransaction(generatedSources);

            try
            {
                transaction.Apply();

                _insideJilwerCompile = true;

                UdonSharpProgramAsset.CompileAllCsPrograms(true, editorBuild);

                WaitForUdonSharpCompiler();
            }
            finally
            {
                _insideJilwerCompile = false;
            }

            return false;
        }

        private static void WaitForUdonSharpCompiler()
        {
            Assembly udonSharpAssembly = typeof(UdonSharpProgramAsset).Assembly;

            Type compilerType = udonSharpAssembly.GetType("UdonSharp.Compiler.UdonSharpCompilerV1");

            if (compilerType == null) throw new InvalidOperationException($"Could not locate UdonSharpCompilerV1.");

            MethodInfo waitMethod = compilerType.GetMethod("WaitForCompile",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (waitMethod == null) throw new InvalidOperationException($"Could not locate UdonSharpCompilerV1.WaitForCompile().");

            waitMethod.Invoke(null, null);
        }
    }
}