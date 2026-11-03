// ReSharper disable ArrangeObjectCreationWhenTypeEvident
using JetBrains.Annotations;
using UnityEngine;

namespace Gravinium.Jilwer.Math
{
    public static class ArrayMath
    {
        /// <summary>
        /// Returns the index for a 1D array from the coordinates of a 2D index.
        /// </summary>
        /// <param name="x">X coord.</param>
        /// <param name="y">Y coord.</param>
        /// <param name="width">Width of the 2D array.</param>
        /// <returns>1D index.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/arraymath#index2dto1d"/>
        [PublicAPI]
        public static int Index2DTo1D(int x, int y, int width) => y * width + x;

        /// <summary>
        /// Returns the index for a 1D array from the coordinates of a 3D index.
        /// </summary>
        /// <param name="x">X coord.</param>
        /// <param name="y">Y coord.</param>
        /// <param name="z">Z coord.</param>
        /// <param name="width">Width of the 3D array.</param>
        /// <param name="height">Height of the 3D array.</param>
        /// <returns>1D index.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/arraymath#index3dto1d"/>
        [PublicAPI]
        public static int Index3DTo1D(int x, int y, int z, int width, int height) => (z * height + y) * width + x;

        /// <summary>
        /// Returns the 2D index from the index of a 1D index.
        /// </summary>
        /// <param name="index">Index of the 1D array.</param>
        /// <param name="width">Width of the 2D array.</param>
        /// <returns>Vector2Int (x, y) index.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/arraymath#index1dto2d"/>
        [PublicAPI]
        public static Vector2Int Index1DTo2D(int index, int width) => new Vector2Int(index % width, index / width);

        /// <summary>
        /// Returns the 3D index from the index of a 1D index.
        /// </summary>
        /// <param name="index">Index of the 1D array.</param>
        /// <param name="width">Width of the 3D array.</param>
        /// <param name="height">Height of the 3D array.</param>
        /// <returns>Vector3Int (x, y, z) index.</returns>
        /// <seealso href="http://docs.gravinium.org/projects/jilwer/api/math/arraymath#index1dto3d"/>
        [PublicAPI]
        public static Vector3Int Index1DTo3D(int index, int width, int height)
            => new Vector3Int(index % width, (index / width) % height, index / (width * height));
    }
}