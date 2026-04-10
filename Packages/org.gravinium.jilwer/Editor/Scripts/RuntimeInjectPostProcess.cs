using System.Reflection;
using Org.Gravinium.Jilwer.Runtime.Core;
using UdonSharp;
using UnityEngine;

namespace Org.Gravinium.Jilwer.Editor
{
    public class RuntimeInjectPostProcess
    {
        public static void InjectRuntime(TypeRegistry registry)
        {
            JilwerRuntime[] runtimes = Object.FindObjectsOfType<JilwerRuntime>(true);

            foreach (var runtime in runtimes)
            {
                runtime.types = registry;
            }
        }

    }
}