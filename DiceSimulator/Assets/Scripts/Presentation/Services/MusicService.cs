using System;
using Core;
using Core.Config;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Presentation.Services
{
    /// <summary>
    /// Music is loaded in and out of RAM memory dynamically when needed.
    /// </summary>
    static class MusicService
    {
        // generated classes does not support pragmas
        // suppression has to be done manually
        static readonly AudioConfig _config = null!;

        internal static int Volume
        {
            set
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Assert.IsFalse(value == _volume, "Assigning the same volume value is not allowed.");
                _volume = value;
#endif

                _config.AudioMixer.SetFloat(Music, Utils.ConvertVolumeToDecibels(value * 10));
            }
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static int _volume = int.MinValue;
#endif

        /// <summary>
        /// If contains value then the audio clip corresponding the given <see cref="Music"/> value is being loaded or is already loaded.
        /// </summary>
        static readonly AudioClip[] _loadedMusic = new AudioClip [Enum.GetNames(typeof(Music)).Length];

        static readonly AsyncOperationHandle<AudioClip>[] _asyncOperationHandles = new AsyncOperationHandle<AudioClip> [Enum.GetNames(typeof(Music)).Length];

        /// <summary>
        /// Contains audio clip's index. If it has value then music is either already loaded or is being loaded into memory, null otherwise.
        /// </summary>
        static Music? _currentMusic;

        const string Music = "musicVolume";
        static AudioSource _musicSource = null!;

        internal static void Initialize() => _musicSource = PresentationSceneReferenceHolder.MusicAudioSource;

        /// <summary>
        /// Plays the music as soon as it is loaded into memory.
        /// </summary>
        internal static void LoadAndPlayWhenReady(Music music, bool unloadPrevious = true)
        {
            int id = Convert.ToInt32(music);

            Assert.IsNull(_loadedMusic[id], "It is invalid to request to load a music when the music is already loaded or is being loaded.");
            Assert.IsTrue(_asyncOperationHandles[id].IsDone, "Operation still in progress.");

            _asyncOperationHandles[id] = _config.Music[id].LoadAssetAsync<AudioClip>();
            _asyncOperationHandles[id].Completed += asyncOperationHandle =>
            {
                _musicSource.Stop();
                _loadedMusic[id] = asyncOperationHandle.Result;
                _musicSource.clip = _loadedMusic[id];
                _musicSource.Play();

                if (unloadPrevious && _currentMusic.HasValue)
                    Unload(_currentMusic.Value);

                _currentMusic = music;
            };
        }

        /// <summary>
        /// Unloads music asset from memory.
        /// Throws an error is requested music is not present in the memory.
        /// </summary>
        internal static void Unload(Music music)
        {
            int id = Convert.ToInt32(music);

            Assert.IsNotNull(_loadedMusic[id], "It is invalid to request to unload a music when the music is already unloaded from memory.");

            _loadedMusic[id] = null;
            _config.Music[id].ReleaseAsset();
        }
    }
}