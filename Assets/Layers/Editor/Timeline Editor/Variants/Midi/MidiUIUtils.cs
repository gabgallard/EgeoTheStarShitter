namespace ABXY.Layers.Editor.Timeline_Editor.Variants.Midi
{
    public static class MidiUIUtils
    {
    

        // FROM https://stackoverflow.com/questions/31997707/rounding-value-to-nearest-power-of-two
        public static int FindNextPowerOf2(int x)
        {
            if (x < 0) { return 0; }
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        // FROM https://stackoverflow.com/questions/31997707/rounding-value-to-nearest-power-of-two
        public static int FindNearestPowerOf2(int x)
        {
            int next = FindNextPowerOf2(x);
            int prev = next >> 1;
            return next - x < x - prev ? next : prev;
        }

        // From https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
        public static bool IsPowerOfTwo(ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }
    }
}
