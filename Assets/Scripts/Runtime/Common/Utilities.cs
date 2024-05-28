﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public static class MathUtilities
    {
        /// <summary>
        /// Attempts to reflectively add two values.
        /// </summary>
        public static T Add<T>(T a, T b)
        {
            var type = typeof(T);
            return type.GetMethod("op_Addition") != null
                ? (T) type.GetMethod("op_Addition")!
                    .Invoke(null, new object[] { a, b })
                : throw new NotSupportedException("Addition is not supported for this type");
        }

        /// <summary>
        /// Attempts to reflectively multiply two values.
        /// </summary>
        public static T Multiply<T>(T a, T b)
        {
            var type = typeof(T);
            return type.GetMethod("op_Multiply") != null
                ? (T) type.GetMethod("op_Multiply")!
                    .Invoke(null, new object[] { a, b })
                : throw new NotSupportedException("Multiplication is not supported for this type");
        }

        /// <summary>
        /// Finds a valid spawning point within a radius of the origin.
        /// </summary>
        /// <param name="origin">The origin point.</param>
        /// <param name="radius">The radius to search.</param>
        /// <param name="isValidSpawn">The function to use to check the spawn validity.</param>
        /// <returns></returns>
        public static Vector2 FindValidSpawn(
            Vector2 origin, float radius,
            Func<Vector2, bool> isValidSpawn)
        {
            Vector3 spawnPoint;
            while (true)
            {
                var point = (Random.insideUnitCircle.normalized * radius).Round();
                spawnPoint = point + origin;

                if (isValidSpawn(spawnPoint)) break;
            }

            return spawnPoint;
        }
    }

    public static class ImageUtilities
    {
        /// <summary>
        /// Constructs a new texture from the given byte array.
        /// </summary>
        public static Texture2D FromBytes(int width, int height, byte[] data)
        {
            var texture = new Texture2D(width, height,
                TextureFormat.RGBA32,
                false, true);
            texture.LoadRawTextureData(data);

            // Flip the texture so it's the right way up.
            var flipped = new Texture2D(width, height,
                TextureFormat.RGBA32,
                false, true);

            var xN = texture.width;
            var yN = texture.height;

            for (var i = 0; i < xN; i++)
            {
                for (var j = 0; j < yN; j++)
                {
                    flipped.SetPixel(j, xN - i - 1, texture.GetPixel(j, i));
                }
            }
            flipped.Apply();

            return flipped;
        }
    }
}
