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

        /// <summary>
        /// Distance from the center of the model to the surface.
        /// </summary>
        [HideInInspector]
        public float Distance;
    }
}