using System;
using System.Linq;
using System.Reflection;
using Gravinium.Jilwer.Core;
using UdonSharp;
using UdonSharpEditor;
using UnityEngine;

namespace Gravinium.Jilwer.Editor.PreProcess
{
    internal class TypeRegistryPreProcess : IPreProcess
    {
        public void Process()
        {
            if (!JilwerBuildState.Root) throw new MissingFieldException("[Jilwer] Root object not found.");
            if (!JilwerBuildState.Runtime) throw new MissingFieldException("[Jilwer] Runtime not found.");
            
            GameObject registryObject = new GameObject("Jilwer__TypeRegistry")
                { transform = { parent = JilwerBuildState.Root.transform } };
            TypeRegistry registryComponent = registryObject.AddUdonSharpComponent<TypeRegistry>();

            GameObject typeObjectParent = new GameObject("Jilwer__TypeObjects") 
                { transform = { parent = registryObject.transform } };
            registryComponent.parentContainer = typeObjectParent;

            var registries = GetAllRegistries();

            foreach (var reg in registries)
            {
                string name = reg.Name;
                GameObject typeObject = new GameObject($"Jilwer__Type_{name}")
                    { transform = { parent = registryObject.transform } };
                typeObject.SetActive(false);
                typeObject.AddUdonSharpComponent(reg);

                registryComponent.objects.Add(name, typeObject);
            }

            JilwerBuildState.Runtime.typeRegistry = registryComponent;
        }
        
        private static Type[] GetAllRegistries()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                }).Where(t => typeof(UdonSharpBehaviour).IsAssignableFrom(t) &&
                              !t.IsAbstract &&
                              t.GetCustomAttribute<JilwerType>() != null).ToArray();
        }
    }
}