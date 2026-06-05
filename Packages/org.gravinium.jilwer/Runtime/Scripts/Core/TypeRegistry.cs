using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Data;

namespace Gravinium.Jilwer.Core
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TypeRegistry : UdonSharpBehaviour
    {
        public DataDictionary objects = new DataDictionary();
        public GameObject parentContainer;
        public int id;

        public static Error Create(JilwerRuntime runtime, string key, out GameObject newObj)
        {
            if (!runtime)
            {
                newObj = null;
                return Error.RuntimeDoesNotExist;
            }

            TypeRegistry reg = runtime.typeRegistry;

            var err = Get(reg, key, out GameObject refObj);
            if (err != Error.None)
            {
                newObj = null;
                return err;
            }
            
            newObj = Instantiate(refObj, reg.parentContainer.transform);
            newObj.name = newObj.name.Replace("(Clone)", "_" + reg.id++).Trim();
            
            return Error.None;
        }

        private static Error Get(TypeRegistry reg, string key, out GameObject obj)
        {
            if (!reg.objects.TryGetValue(key, out DataToken objToken))
            {
                obj = null;
                return Error.KeyValueNotFound;
            }

            obj = (GameObject) objToken.Reference;
            return Error.None;
        }

    }
}