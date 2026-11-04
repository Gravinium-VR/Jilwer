using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using VRC.SDKBase;

namespace Gravinium.Jilwer.VRC
{
    public static class NetworkUtilities
    {
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetOwner(VRCPlayerApi player, GameObject obj)
        {
            if (Networking.IsOwner(player, obj)) return;
            Networking.SetOwner(player, obj);
        }
        
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TakeOwner(GameObject obj)
        {
            #if UNITY_EDITOR
            while (Networking.LocalPlayer == null) {}
            #endif
            if (Networking.IsOwner(obj)) return;
            Networking.SetOwner(Networking.LocalPlayer, obj);
        }
    }
}