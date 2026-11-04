#if UNITY_EDITOR
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Gravinium.Jilwer.VRC
{
    public class ObjectPoolSpawner : MonoBehaviour
    {
        public VRCObjectPool objectPool;
    }
}
#endif