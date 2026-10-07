using System;
using System.Collections.Generic;
using System.IO;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace Gravinium.Jilwer.Editor.Compiler
{
    internal static class JilwerCompilationTargets
    {
        public static HashSet<string> GetLoadedSceneScripts()
        {
            HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase);

            UdonSharpBehaviour[] behaviours =
                UnityEngine.Object.FindObjectsByType<UdonSharpBehaviour>(FindObjectsInactive.Include,
                    FindObjectsSortMode.None);

            foreach (UdonSharpBehaviour behaviour in behaviours)
            {
                MonoScript script = MonoScript.FromMonoBehaviour(behaviour);
                
                if (script == null) continue;

                string path = AssetDatabase.GetAssetPath(script);
                
                if (string.IsNullOrEmpty(path)) continue;

                paths.Add(NormalizePath(path));
            }

            return paths;
        }

        public static string NormalizePath(string path) => Path.GetFullPath(path).Replace('\\', '/');
    }
}