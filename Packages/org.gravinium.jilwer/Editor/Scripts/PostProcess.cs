using Gravinium.Jilwer.Core;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    public class PostProcess : MonoBehaviour
    {
        [PostProcessScene(-10)]
        public static void OnPostProcessScene()
        {
            Debug.Log("[Jilwer] Starting post processing...");

            GameObject JilwerObject = new GameObject("Jilwer");
            Debug.Log("[Jilwer] Parent object created");

            Debug.Log("[Jilwer] Running post process functions");
            TypeRegistry registry = TypeRegistryPostProcess.CreateRegistryObjects(JilwerObject);
            Debug.Log("[Jilwer] Post process functions finished");

            Debug.Log("[Jilwer] Injecting runtime into objects");
            RuntimeInjectPostProcess.InjectRuntime(JilwerObject, registry);

            Debug.Log("[Jilwer] Post processing complete!");
        }
    }
}