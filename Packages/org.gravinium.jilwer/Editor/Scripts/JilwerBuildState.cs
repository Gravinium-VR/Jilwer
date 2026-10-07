using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    internal static class JilwerBuildState
    {
        public static GameObject Root;
        public static GameObject RuntimeObject;

        public static void Clear()
        {
            Root = null;
            RuntimeObject = null;
        }
    }
}