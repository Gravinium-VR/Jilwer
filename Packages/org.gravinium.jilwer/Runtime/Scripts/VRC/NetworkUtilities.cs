using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using VRC.SDKBase;

namespace Gravinium.Jilwer.VRC
{
    public static class NetworkUtilities
    {
        /// <summary>
        /// Attempts to set the owner of an object to a target player.
        /// </summary>
        /// <param name="player">Target player to become owner.</param>
        /// <param name="obj">Object to take ownership of.</param>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/VRC/networkutilities#setowner"/>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetOwner(VRCPlayerApi player, GameObject obj)
        {
            if (Networking.IsOwner(player, obj)) return;
            Networking.SetOwner(player, obj);
        }
        
        /// <summary>
        /// Attempts to set the owner of an object to the local player.
        /// This method has an editor-only guard when the local player is null.
        /// </summary>
        /// <param name="obj">Object to take ownership of.</param>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/VRC/networkutilities#takeowner"/>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TakeOwner(GameObject obj)
        {
            #if UNITY_EDITOR
            if (Networking.LocalPlayer == null)
            {
                Debug.LogError("[Jilwer] LocalPlayer caught as null in 'NetworkUtilities.TakeOwner'.");
                return;
            }
            #endif
            if (Networking.IsOwner(obj)) return;
            Networking.SetOwner(Networking.LocalPlayer, obj);
        }
    }
}