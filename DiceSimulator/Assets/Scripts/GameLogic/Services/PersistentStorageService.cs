#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine;

namespace GameLogic.Services
{
    static class PersistentStorageService
    {
        internal static void Initialize()
        {
            const string MusicKey = nameof(PersistentStorageKey.Music);
            if (!PlayerPrefs.HasKey(MusicKey))
                PlayerPrefs.SetInt(MusicKey, 7);

            const string SoundKey = nameof(PersistentStorageKey.Sound);
            if (!PlayerPrefs.HasKey(SoundKey))
                PlayerPrefs.SetInt(SoundKey, 7);
        }

        internal static (int music, int sound) LoadVolumeSettings() =>
            (PlayerPrefs.GetInt(nameof(PersistentStorageKey.Music)),
             PlayerPrefs.GetInt(nameof(PersistentStorageKey.Sound)));

        internal static void SaveVolumeSettings(int music, int sound)
        {
            PlayerPrefs.SetInt(nameof(PersistentStorageKey.Music), music);
            PlayerPrefs.SetInt(nameof(PersistentStorageKey.Sound), sound);
        }
    }
}