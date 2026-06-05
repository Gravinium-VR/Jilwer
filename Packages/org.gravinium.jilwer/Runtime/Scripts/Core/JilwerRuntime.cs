using UdonSharp;

namespace Gravinium.Jilwer.Core
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class JilwerRuntime : UdonSharpBehaviour
    {
        public TypeRegistry typeRegistry;
    }
}