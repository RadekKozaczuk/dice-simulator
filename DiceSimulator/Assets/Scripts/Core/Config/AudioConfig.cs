#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Config/Presentation/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField]
        public AudioSource AudioSourcePrefab;

        [SerializeField]
        public AudioMixer AudioMixer;

        [SerializeField]
        public AudioMixerGroup AudioMixerSound;

        [Space(15)]
        [InfoBox("Element order must match the Sound enum.", InfoMessageType.None)]
        [SerializeField]
        public SoundData[] Sounds;

        [Space(15)]
        [InfoBox("Element order must match the Music enum.", InfoMessageType.None)]
        [SerializeField]
        public AssetReferenceAudioClip[] Music;

        [Min(0)]
        [InfoBox("Maximum number of the AudioSource game objects stored in the pool.", InfoMessageType.None)]
        [SerializeField]
        public int SoundPoolSize = 5;
    }
}