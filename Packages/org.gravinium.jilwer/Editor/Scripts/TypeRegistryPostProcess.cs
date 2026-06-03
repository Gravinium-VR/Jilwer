using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gravinium.Jilwer.Core;
using UdonSharp;
using UdonSharpEditor;
using UnityEngine;


namespace Gravinium.Jilwer.Editor
{
    public static class TypeRegistryPostProcess
    {
        public static TypeRegistry CreateRegistryObjects(GameObject jilwerObject)
        {
            GameObject registryObject = new GameObject("Jilwer__TypeRegistry")
                { transform = { parent = jilwerObject.transform } };
            TypeRegistry registryComponent = registryObject.AddUdonSharpComponent<TypeRegistry>();

            GameObject typeObjectParent = new GameObject("Jilwer__TypeObjects") 
                { transform = { parent = registryObject.transform } };
            registryComponent.parentContainer = typeObjectParent;

            var registries = GetAllRegistries();

            var keyList = new List<string>();
            var objectList = new List<GameObject>();

            foreach (var reg in registries)
            {
                string name = reg.Name;
                keyList.Add(name);

                GameObject typeObject = new GameObject($"Jilwer__Type_{name}")
                    { transform = { parent = registryObject.transform } };

                typeObject.AddUdonSharpComponent(reg);

                objectList.Add(typeObject);
            }

            registryComponent.keys = keyList.ToArray();
            registryComponent.objects = objectList.ToArray();

            return registryComponent;
        }

        private static Type[] GetAllRegistries()
        {
            // AI: I have zero clue how this works. I'll eventually get to rewriting this on my own, but until then,
            // this comment will remain here. (Please give me Rust iterators T-T)
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                }).Where(t =>
                    typeof(UdonSharpBehaviour).IsAssignableFrom(t) && !t.IsAbstract &&
                    t.GetCustomAttribute<JilwerType>() != null).ToArray();
        }
        
    }

}