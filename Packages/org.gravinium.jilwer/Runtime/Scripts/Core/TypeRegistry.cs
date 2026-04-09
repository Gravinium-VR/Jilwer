using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Org.Gravinium.Jilwer.Runtime.Core
{
    public class TypeRegistry : UdonSharpBehaviour
    {
        public string[] keys = new string[0];
        public GameObject[] objects = new GameObject[0];
        public GameObject parentContainer = null;
        public int id = 0;

        public static GameObject Create(string key)
        {
            TypeRegistry reg = GameObject.Find("Jilwer__TypeRegistry").GetComponent<TypeRegistry>();

            GameObject newObj = Instantiate(Get(reg, key));
            newObj.transform.parent = reg.parentContainer.transform;
            newObj.name = newObj.name.Replace("(Clone)", "").Trim();
            newObj.name += "_" + reg.id;
            reg.id++;
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