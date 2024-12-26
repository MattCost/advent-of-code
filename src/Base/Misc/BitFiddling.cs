using System.Runtime.InteropServices;

namespace AdventOfCode.Base.Misc
{
    public static class BitFiddlingExtensions
    {
        /// <summary>
        /// If a single bit is set, returns the bit number, if multiple bits are set, returns -1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetSingleBitSet(this long input)
        {
            for (int i = 0; i < 64; i++)
            {
                if (((1L << i) & input) == (1L << i))
                    return i;

            }
            return -1;
        }
    }
}