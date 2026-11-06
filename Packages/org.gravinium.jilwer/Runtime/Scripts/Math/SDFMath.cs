using JetBrains.Annotations;
using UnityEngine;

namespace Gravinium.Jilwer.Math
{
    public static class SDFMath
    {
        /// <summary>
        /// Returns the distance from the edge of a sphere.
        /// </summary>
        /// <param name="position">Position to evaluate.</param>
        /// <param name="center">Center of the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <returns>Positive if outside, zero if on the surface, negative if inside.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/sdfmath#sdsphere"/>
        [PublicAPI]
        public static float SDSphere(Vector3 position, Vector3 center, float radius)
            => (position - center).magnitude - radius;
        
        /// <summary>
        /// Returns the distance from the edge of a box.
        /// </summary>
        /// <param name="position">Position to evaluate.</param>
        /// <param name="center">Center of the box.</param>
        /// <param name="size">Size of the box in world space.</param>
        /// <returns>Positive if outside, zero if on the surface, negative if inside.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/sdfmath#sdbox"/>
        [PublicAPI]
        public static float SDBox(Vector3 position, Vector3 center, Vector3 size)
        {
            Vector3 p = position - center;
            Vector3 q = VectorMath.Abs(p) - size * 0.5f;

            float outside = VectorMath.Max(q, Vector3.zero).magnitude;
            float inside = Mathf.Min(Mathf.Max(q.x, Mathf.Max(q.y, q.z)), 0f);

            return outside + inside;
        }
    }
}