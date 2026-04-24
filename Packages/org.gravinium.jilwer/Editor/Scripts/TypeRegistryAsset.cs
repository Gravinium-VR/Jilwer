using System;
using UnityEditor;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    [Serializable]
    [CreateAssetMenu(menuName = "Gravinium/Jilwer/Type Registry")]
    public class TypeRegistryAsset : ScriptableObject
    {
        public MonoScript[] scripts;
    }
}