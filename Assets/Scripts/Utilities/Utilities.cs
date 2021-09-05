using System.Collections.Generic;
using UnityEngine;

namespace LUX.LightOfHeaven {
    public static class Utilities {
        public static Vector3 SpawnIsoOffset = new Vector3(0,1,0);
        public static Vector3 ToIsoPos(this Vector3 v) {
            return new Vector3(
                v.x + v.y,
                .5f * (v.y - v.x),
                0
            );
        }
        public static int BitmaskToLayerInt(int bitMask) {
            int result = bitMask > 0 ? 0 : 31;
            while (bitMask > 1) {
                bitMask = bitMask >> 1;
                result++;
            }
            return result;
        }
        public static void ShuffleArray<T>(this System.Random rng, T[] array) {
            int n = array.Length;
            while (n > 1) {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}