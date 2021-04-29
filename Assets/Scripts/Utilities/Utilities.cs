using UnityEngine;

namespace LUX {
    public static class Utilities {
        public static int BitmaskToLayerInt(int bitMask) {
            int result = bitMask > 0 ? 0 : 31;
            while (bitMask > 1) {
                bitMask = bitMask >> 1;
                result++;
            }
            return result;
        }
    }
}