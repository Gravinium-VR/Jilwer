using System.Diagnostics;
using Gravinium.Jilwer.Editor.PostProcess;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;

namespace Gravinium.Jilwer.Editor
{
    internal static class JilwerPostProcess
    {
        private static readonly IPostProcess[] Passes =
        {
            new InjectRuntimePostProcess(),
        };
        
        [PostProcessScene(10000)]
        private static void PostProcess()
        {
            Debug.Log($"[Jilwer] Running PostProcess.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            foreach (IPostProcess pass in Passes) pass.Process();
            
            JilwerBuildState.Clear();
            
            stopwatch.Stop();
            Debug.Log($"[Jilwer] Finished PostProcess in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}