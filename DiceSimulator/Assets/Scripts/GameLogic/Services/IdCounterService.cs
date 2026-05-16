#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Core;

namespace GameLogic.Services
{
    static class IdCounterService
    {
        internal static int NextId(BrickType type) => _counters[(int)type]++;

        static readonly int[] _counters = new int[Enum.GetNames(typeof(BrickType)).Length];
    }
}