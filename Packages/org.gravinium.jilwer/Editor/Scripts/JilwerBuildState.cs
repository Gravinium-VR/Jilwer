using Gravinium.Jilwer.Core;
using UnityEngine;

namespace Gravinium.Jilwer.Editor
{
    internal static class JilwerBuildState
    {
        public static GameObject Root;
        public static GameObject RuntimeObject;
        public static JilwerRuntime Runtime;

        public static void Clear()
        {
            Root = null;
            RuntimeObject = null;
            Runtime = null;
        }
    }
}