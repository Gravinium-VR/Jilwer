using System.Reflection;
using Org.Gravinium.Jilwer.Runtime.Core;
using UdonSharp;
using UdonSharpEditor;
using UnityEngine;

namespace Org.Gravinium.Jilwer.Editor
{
    public class RuntimeInjectPostProcess
    {
        public static void InjectRuntime(GameObject JilwerObject, TypeRegistry registry)
        {
            Debug.Log("[Jilwer] Creating runtime");
            GameObject runtimeObj = new GameObject("Jilwer__Runtime");
            runtimeObj.transform.parent = JilwerObject.transform;

            JilwerRuntime runtimeScript = runtimeObj.AddUdonSharpComponent<JilwerRuntime>();
            runtimeScript.types = registry;

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