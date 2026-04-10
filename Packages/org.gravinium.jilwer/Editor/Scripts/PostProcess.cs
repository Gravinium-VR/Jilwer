using Org.Gravinium.Jilwer.Runtime.Core;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Org.Gravinium.Jilwer.Editor
{
    public class PostProcess : MonoBehaviour
    {
        [PostProcessScene(-10)]
        public static void OnPostProcessScene()
        {
            Debug.Log("[Jilwer] Starting post processing...");

            GameObject JilwerObject = new GameObject("Jilwer");
            Debug.Log("[Jilwer] Parent object created!");

            Debug.Log("[Jilwer] Running post process functions...");
            TypeRegistry registry = TypeRegistryPostProcess.CreateRegistryObjects(JilwerObject);

            /*
            TODO: Current issue

            The Runtime doesn't exist. An object needs to be created and the registry assigned to it.
            Then the object can be added to any object that needs it.
            */

            RuntimeInjectPostProcess.InjectRuntime(registry);
            Debug.Log("[Jilwer] Post process functions complete!");

            Debug.Log("[Jilwer] Post processing complete!");
        }
    }
}