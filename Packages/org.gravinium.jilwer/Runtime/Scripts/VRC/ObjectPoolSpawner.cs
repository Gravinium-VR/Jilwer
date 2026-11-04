#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Components;

namespace Gravinium.Jilwer.VRC
{
    [ExecuteInEditMode]
    public class ObjectPoolSpawner : MonoBehaviour
    {
        public VRCObjectPool objectPool;
        public GameObject referenceObject;
        public int copies = 1;
    }
}
#endif