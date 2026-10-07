using System.Linq;
using Gravinium.Jilwer.Editor.Compiler.Passes;
using UnityEditor.Callbacks;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common;

namespace Gravinium.Jilwer.Editor
{
    internal static class JilwerPostProcess
    {
        [PostProcessScene(10000)]
        private static void BindGeneratedState()
        {
            GameObject runtimeObject = JilwerBuildState.RuntimeObject;

            if (!runtimeObject)
            {
                Debug.LogError($"[Jilwer] Runtime object disappeared during scene processing.");
                return;
            }

            UdonBehaviour runtimeBacking = runtimeObject.GetComponents(typeof(UdonBehaviour)).OfType<UdonBehaviour>()
                .FirstOrDefault();

            if (runtimeBacking == null)
            {
                Debug.LogError($"[Jilwer] Runtime backing UdonBehaviour was not generated.");
                return;
            }

            UdonBehaviour[] behaviours =
                Object.FindObjectsOfType(typeof(UdonBehaviour), true).OfType<UdonBehaviour>().ToArray();

            int injected = 0;

            foreach (UdonBehaviour behaviour in behaviours)
            {
                if (behaviour == runtimeBacking) continue;

                var program = behaviour.programSource?.SerializedProgramAsset?.RetrieveProgram();

                if (program == null) continue;
                
                if (!program.SymbolTable.HasAddressForSymbol(RuntimeStatePass.RuntimeFieldName)) continue;

                if (behaviour.publicVariables.TrySetVariableValue(RuntimeStatePass.RuntimeFieldName, runtimeBacking))
                {
                    injected++;
                    Debug.Log($"[Jilwer] Updated runtime on '{behaviour.gameObject.name}'.");
                    continue;
                }

                behaviour.publicVariables.TryAddVariable(
                    new UdonVariable<UdonBehaviour>(RuntimeStatePass.RuntimeFieldName, runtimeBacking));
                injected++;
                
                Debug.Log($"[Jilwer] Added generated runtime variable to '{behaviour.gameObject.name}'.");
            }
            
            Debug.Log($"[Jilwer] Runtime injected into {injected} behaviour(s).");
            
            JilwerBuildState.Clear();
        }
    }
}