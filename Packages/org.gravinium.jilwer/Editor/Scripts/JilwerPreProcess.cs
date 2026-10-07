using Gravinium.Jilwer.Core;
using UdonSharpEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    internal static class JilwerPreProcess
    {
        [PostProcessScene(-10000)]
        private static void PrepareScene()
        {
            JilwerBuildState.Clear();
            
            Debug.Log($"[Jilwer] Preparing runtime objects.");

            GameObject root = new GameObject("Jilwer");

            TypeRegistry registry = TypeRegistryPostProcess.CreateRegistryObjects(root);

            GameObject runtimeObject = new GameObject("Jilwer__Runtime")
            {
                transform = { parent = root.transform }
            };

            JilwerRuntime runtime = runtimeObject.AddUdonSharpComponent<JilwerRuntime>();

            runtime.typeRegistry = registry;

            JilwerBuildState.Root = root;
            JilwerBuildState.RuntimeObject = runtimeObject;
        }
    }
}