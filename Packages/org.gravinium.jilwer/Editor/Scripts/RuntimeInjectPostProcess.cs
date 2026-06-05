using System.Reflection;
using Gravinium.Jilwer.Core;
using UdonSharp;
using UdonSharpEditor;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    public static class RuntimeInjectPostProcess
    {
        public static void InjectRuntime(GameObject jilwerObject, TypeRegistry registry)
        {
            Debug.Log("[Jilwer] Creating runtime");
            GameObject runtimeObj = new GameObject("Jilwer__Runtime") { transform = { parent = jilwerObject.transform } };

            JilwerRuntime runtimeScript = runtimeObj.AddUdonSharpComponent<JilwerRuntime>();
            runtimeScript.typeRegistry = registry;

            var allComponents = Object.FindObjectsOfType<UdonSharpBehaviour>(true);
            foreach (var component in allComponents)
            {
                var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(JilwerRuntime))
                    {
                        field.SetValue(component, runtimeScript);
                    }
                }
            }
        }
    }
}