// ReSharper disable ArrangeObjectCreationWhenTypeEvident
using JetBrains.Annotations;
using UnityEngine;

namespace Gravinium.Jilwer.Math
{
    public static class VectorMath
    {
        // Abs
        /// <summary>
        /// Absolute values of the vector.
        /// </summary>
        /// <param name="vec">Original vector.</param>
        /// <returns>Vector2 of absolute values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#abs"/>
        [PublicAPI]
        public static Vector2 Abs(Vector2 vec) => new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
        
        /// <summary>
        /// Absolute values of the vector.
        /// </summary>
        /// <param name="vec">Original vector.</param>
        /// <returns>Vector3 of absolute values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#abs"/>
        [PublicAPI]
        public static Vector3 Abs(Vector3 vec) => new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
        
        /// <summary>
        /// Absolute values of the vector.
        /// </summary>
        /// <param name="vec">Original vector.</param>
        /// <returns>Vector4 of absolute values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#abs"/>
        [PublicAPI]
        public static Vector4 Abs(Vector4 vec) =>
            new Vector4(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z), Mathf.Abs(vec.w));

        // Max
        /// <summary>
        /// Maximum values of the vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector2 of maximum values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#max"/>
        [PublicAPI]
        public static Vector2 Max(Vector2 a, Vector2 b) => new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        
        /// <summary>
        /// Maximum values of the vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector3 of maximum values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#max"/>
        [PublicAPI]
        public static Vector3 Max(Vector3 a, Vector3 b) => 
            new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        
        /// <summary>
        /// Maximum values of the vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector4 of maximum values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#max"/>
        [PublicAPI]
        public static Vector4 Max(Vector4 a, Vector4 b) => 
            new Vector4(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z), Mathf.Max(a.w, b.w));
        
        // Min
        /// <summary>
        /// Minimum values of the vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector2 of minimum values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#min"/>
        [PublicAPI]
        public static Vector2 Min(Vector2 a, Vector2 b) => new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
        
        /// <summary>
        /// Minimum values of the vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector3 of minimum values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#min"/>
        [PublicAPI]
        public static Vector3 Min(Vector3 a, Vector3 b) => 
            new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        
        /// <summary>
        /// Minimum values of the vector.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Vector4 of minimum values.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/vectormath#min"/>
        [PublicAPI]
        public static Vector4 Min(Vector4 a, Vector4 b) => 
            new Vector4(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z), Mathf.Min(a.w, b.w));
    }
}