using System.Diagnostics;
using Gravinium.Jilwer.Core;
using Gravinium.Jilwer.Editor.PreProcess;
using UdonSharpEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Gravinium.Jilwer.Editor
{
    internal static class JilwerPreProcess
    {
        private static readonly IPreProcess[] Passes =
        {
            new TypeRegistryPreProcess(),
            new ObjectPoolSpawnerPreProcess(),
        };
        
        [PostProcessScene(-10000)]
        private static void PreProcess()
        {
            Debug.Log($"[Jilwer] Running PreProcess.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            JilwerBuildState.Clear();

            GameObject root = new GameObject("Jilwer");
            JilwerBuildState.Root = root;

            GameObject runtimeObject = new GameObject("Jilwer__Runtime")
                { transform = { parent = root.transform } };
            JilwerBuildState.RuntimeObject = runtimeObject;
            
            JilwerRuntime runtime = runtimeObject.AddUdonSharpComponent<JilwerRuntime>();
            JilwerBuildState.Runtime = runtime;
            
            foreach (IPreProcess pass in Passes) pass.Process();
            
            stopwatch.Stop();
            Debug.Log($"[Jilwer] Finished PreProcess in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}