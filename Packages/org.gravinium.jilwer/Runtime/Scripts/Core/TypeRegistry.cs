using System;
using UdonSharp;
using UnityEngine;

namespace Gravinium.Jilwer.Core
{
    public class TypeRegistry : UdonSharpBehaviour
    {
        public string[] keys = Array.Empty<string>();
        public GameObject[] objects = Array.Empty<GameObject>();
        public GameObject parentContainer;
        public int id;

        public static GameObject Create(JilwerRuntime runtime, string key)
        {
            if (!runtime)
            {
                Debug.LogError("[Jilwer] Runtime not set!");
                return null;
            }

            TypeRegistry reg = runtime.types;

            GameObject newObj = Instantiate(Get(reg, key), reg.parentContainer.transform);
            newObj.name = newObj.name.Replace("(Clone)", "_" + reg.id++).Trim();

            return newObj;
        }

        private static GameObject Get(TypeRegistry reg, string key)
        {
            int len = reg.keys.Length;
            for (int i = 0; i < len; i++)
            {
                if (!reg.keys[i].Equals(key)) continue;

                return reg.objects[i];
            }

            Debug.LogError("[Jilwer] Could not find key in TypeRegistry");
            return null;
        }

    }
}