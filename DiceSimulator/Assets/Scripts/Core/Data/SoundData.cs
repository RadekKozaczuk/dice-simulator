#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(menuName = "Data/Presentation/SoundData")]
    public class SoundData : ScriptableObject
    {
        [Serializable]
        public struct SoundPair
        {
            [TableColumnWidth(200)]
            [SerializeField]
            public AudioClip Sound;
        }

        [InfoBox("Each probability must be a value from 1 to 100, and their sum must be equal 100.", InfoMessageType.None)]
        [TableList]
        [SerializeField]
        public SoundPair[] Sounds;

        [TableColumnWidth(10)]
        [Range(0, 100)]
        [SuffixLabel("%")]
        [SerializeField]
        public int Volume = 100;
    }
}