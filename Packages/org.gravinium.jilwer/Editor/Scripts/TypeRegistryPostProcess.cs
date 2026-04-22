using System;
using System.Collections.Generic;
using Org.Gravinium.Jilwer.Runtime.Core;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Org.Gravinium.Jilwer.Editor
{
    public class TypeRegistryPostProcess
    {
        public static TypeRegistry CreateRegistryObjects(GameObject jilwerObject)
        {
            GameObject registryObject = new GameObject("Jilwer__TypeRegistry");
            registryObject.transform.parent = jilwerObject.transform;
            TypeRegistry registryComponent = registryObject.AddUdonSharpComponent<TypeRegistry>();

            GameObject typeObjectParent = new GameObject("Jilwer__TypeObjects");
            typeObjectParent.transform.parent = registryObject.transform;
            registryComponent.parentContainer = typeObjectParent;

            TypeRegistryAsset[] registries = GetAllRegistries();

            List<string> keyList = new List<string>();
            List<GameObject> objectList = new List<GameObject>();

            foreach (var reg in registries)
            {
                foreach (var script in reg.scripts)
                {
                    string name = script.GetClass().Name;
                    keyList.Add(name);

                    GameObject typeObject = new GameObject($"Jilwer__Type_{name}");
                    typeObject.transform.parent = registryObject.transform;

                    typeObject.AddUdonSharpComponent(script.GetClass());

                    objectList.Add(typeObject);
                }
            }

            registryComponent.keys = keyList.ToArray();
            registryComponent.objects = objectList.ToArray();

            return registryComponent;
        }

        private static TypeRegistryAsset[] GetAllRegistries()
        {
            string[] guids = AssetDatabase.FindAssets("t:TypeRegistryAsset");

            TypeRegistryAsset[] registries = new TypeRegistryAsset[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                registries[i] = AssetDatabase.LoadAssetAtPath<TypeRegistryAsset>(path);
            }

            return registries;
        }

    }

}