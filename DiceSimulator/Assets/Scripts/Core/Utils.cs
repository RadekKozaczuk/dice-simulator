using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Core
{
    public static class Utils
    {
        public static bool HasDuplicates(int[] array)
        {
            var set = new HashSet<int>();
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < array.Length; i++)
                if (!set.Add(array[i]))
                    return true;

            return false;
        }

        /// <summary>
        /// Converts volume value represented by a human-readable integer [0, 100] to a Decibel (dB) value [-80, 0].
        /// Zero is mapped to - 80.
        /// Values from 1 to 100 are mapped to [-30, 0] in a linear fashion.
        /// In Unity all sounds and music are played with at a certain volume, and we can only lower this value.
        /// That's why this function returns only negative values.
        /// </summary>
        public static int ConvertVolumeToDecibels(int volume)
        {
            Assert.IsTrue(volume is >= 0 and <= 100,
                "All volume values must be >= 0 and <= 100. "
                + "If your value is presented to players differently you should map it to 0, 100 before using this function.");

            if (volume == 0)
                return -80;

            return (int)(-33.3f + volume * .33f);
        }
    }
}