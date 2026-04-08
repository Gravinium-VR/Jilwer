using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Org.Gravinium.Jilwer.Runtime.Core
{
    public class ObjectRegistry : UdonSharpBehaviour
    {
        // Object Type IDs
        public const int ObjectArrayList = 0;

        // ID -> Pointer (Index)
        public int[] objPointer = new int[0];
        // Pointer (Index) -> GameObject
        public GameObject[] objRegistry = new GameObject[0];

        public static GameObject Get(int id)
        {
            ObjectRegistry reg = GameObject.Find("Jilwer__ObjectRegistry").GetComponent<ObjectRegistry>();
            // Object reference not set to an instance of an object
            return reg.objRegistry[reg.objPointer[id]];
        }

    }
}