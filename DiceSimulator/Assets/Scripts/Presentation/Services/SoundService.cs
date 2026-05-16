#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using Core.Pooling;
using Core;
using Core.Config;
using Core.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Presentation.Services
{
    /// <summary>
    /// Sounds are always present in the memory and therefore no need to be loaded/unloaded.
    /// </summary>
    static class SoundService
    {
        // generated classes does not support pragmas
        // suppression has to be done manually
        static readonly AudioConfig _config;

        internal static int Volume
        {
            set
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Assert.IsFalse(value == _volume, "Assigning the same volume value is not allowed.");
                _volume = value;
#endif

                _config.AudioMixer.SetFloat(Sound, Utils.ConvertVolumeToDecibels(value * 10));
            }
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static int _volume = int.MinValue;
#endif

        static readonly ObjectPool<AudioSource> _pool = new(CustomAlloc, null, CustomReturn, 5);
        static Vector3 _position;
        static Transform _audioContainer = null!;
        const string Sound = "soundVolume";

        /// <summary>
        /// Looped sounds must be manually stopped.
        /// </summary>
        static readonly Dictionary<int, AudioSource> _loopedSounds = new();

        internal static void Initialize()
        {
            _audioContainer = PresentationSceneReferenceHolder.AudioContainer;
            _pool.MaxSize = _config.SoundPoolSize;
        }

        /// <summary>
        /// Immediately plays one of the audio clip from the corresponding <see cref="SoundData"/> at 0,0,0 position.
        /// The audio clip is chosen at random taking into account its probability.
        /// Final volume is taken from the sound data and <see cref="Volume"/>.
        /// </summary>
        internal static void Play(Sound sound) => Play_Internal(sound, Vector3.zero, false);

        /// <summary>
        /// Returns Sound ID.
        /// </summary>
        static int Play_Internal(Sound sound, Vector3 position, bool loop)
        {
            _position = position;
            AudioSource source = _pool.Get();

            SoundData data = _config.Sounds[Convert.ToInt32(sound)];
            SoundData.SoundPair[] sounds = data.Sounds;

            Assert.IsTrue(sounds.Length > 0,
                "You cannot invoke PlaySound function for a sound data that doesn't have any audio clips."
                + " Add at least one audio clip to the corresponding ScriptableObject.");

            AudioClip clip = sounds[0].Sound;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Assert.IsNotNull(clip, "Selected audio clip cannot be null. Please ensure that all clips added to the sound data have valid references.");
#endif

            source.clip = clip;
            source.outputAudioMixerGroup = _config.AudioMixerSound;
            source.gameObject.SetActive(true);
            source.volume = (float)data.Volume / 100;
            source.loop = loop;
            source.Play();

            int soundId = int.MinValue;

            if (loop)
            {
                soundId = _loopedSounds.Count + 1;
                _loopedSounds.Add(soundId, source);
            }

            return soundId;
        }

        static AudioSource CustomAlloc() =>
            Object.Instantiate(_config.AudioSourcePrefab, _position, Quaternion.identity, _audioContainer);

        static void CustomReturn(AudioSource source, bool poolMaxOut)
        {
            if (poolMaxOut)
                Object.Destroy(source.gameObject);
            else
                source.gameObject.SetActive(false);
        }
    }
}