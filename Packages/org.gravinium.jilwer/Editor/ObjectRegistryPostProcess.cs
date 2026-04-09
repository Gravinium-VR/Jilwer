using System;
using System.Linq;
using Org.Gravinium.Jilwer.Runtime.Core;
using Org.Gravinium.Jilwer.Runtime.Core.Collections;
using UnityEngine;
using VRC.Udon;

namespace Org.Gravinium.Jilwer.Editor
{
    public class ObjectRegistryPostProcess
    {
        private static Type[] _types = {
            typeof(ObjectArrayList)
        };
        private static int[] _ids = {
            ObjectRegistry.ObjectArrayList
        };

        public static void CreateRegistryObject(GameObject jilwerObject)
        {
            GameObject registryObject = new GameObject("Jilwer__ObjectRegistry", typeof(ObjectRegistry));
            registryObject.transform.parent = jilwerObject.transform;
            ObjectRegistry registry = registryObject.GetComponent<ObjectRegistry>();

            GameObject testObj = new GameObject("Jilwer Test Object", typeof(UdonBehaviour));
            UdonBehaviour ub = testObj.GetComponent<UdonBehaviour>();

            registry.objRegistry = new GameObject[_ids.Max() + 1];
            registry.objPointer = new int[_ids.Max() + 1];

            for (int i = 0; i < _types.Length; i++)
            {
                GameObject newObj = new GameObject("Jilwer_Type__" + _ids[i], _types[i]);
                newObj.transform.parent = registryObject.transform;
                registry.objPointer[_ids[i]] = i;
                registry.objRegistry[i] = newObj;
            }
        }
    }
}