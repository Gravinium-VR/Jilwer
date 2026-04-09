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
            TypeRegistryPostProcess.CreateRegistryObjects(JilwerObject);
            Debug.Log("[Jilwer] Post process functions complete!");

            Debug.Log("[Jilwer] Post processing complete!");
        }
    }
}