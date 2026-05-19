using System.Collections.Generic;
using Core.Dtos;
using UnityEngine;

// ReSharper disable UnusedMemberInSuper.Global

namespace Core
{
    public interface ISignal
    {
        void DicePositionChanged(Vector3 position, Quaternion rotation);

        void DiceStopped(int result, int total);

        void DiceSpawned(Vector3 position, Quaternion rotation, List<DiceFace> faces);
    }
}