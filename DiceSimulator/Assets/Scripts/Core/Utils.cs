#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Entity consists of two variables: index and version, and 2 entities are considered as equal only when both index and version are equal.
        /// The reason is that internally some information about the entity are stored in an array - that's why the index is for.
        /// After an entity is destroyed that slot can be reused by another entity.
        /// In order to differentiate entities that hold the same slot another variable was introduced: version.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long EntityIdToLongId(int entityIndex, int entityVersion) =>
            (long)(uint)entityIndex << 32 | (uint)entityVersion; // todo: something is wrong here

        /// <summary>
        /// Entity consists of two variables: index and version, and 2 entities are considered as equal only when both index and version are equal.
        /// The reason is that internally some information about the entity are stored in an array - that's why the index is for.
        /// After an entity is destroyed that slot can be reused by another entity.
        /// In order to differentiate entities that hold the same slot another variable was introduced: version.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int entityIndex, int entityVersion) LongIdToEntityId(long id) =>
            ((int)(id >> 32), (int)(id & 0xFFFFFFFF));
    }
}