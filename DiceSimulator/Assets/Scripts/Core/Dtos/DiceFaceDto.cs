using System;
using UnityEngine;

namespace Core.Dtos
{
    [Serializable]
    public struct DiceFace
    {
        public int Number;

        [HideInInspector]
        public Vector3 Normal;
    }
}