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