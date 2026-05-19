using UnityEngine;

namespace Presentation
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER Core scene is fully loaded up.
    /// </summary>
    class PresentationSceneReferenceHolder : MonoBehaviour
    {
        internal static Transform AudioContainer;
        internal static Camera MainMenuCamera;
        internal static Camera GameplayCamera;
        internal static AudioSource MusicAudioSource;

        [SerializeField]
        Transform _audioContainer;

        [SerializeField]
        Camera _mainCamera;

        [SerializeField]
        Camera _gameplayCamera;

        [SerializeField]
        AudioSource _musicAudioSource;

        void Awake()
        {
            AudioContainer = _audioContainer;
            MainMenuCamera = _mainCamera;
            GameplayCamera = _gameplayCamera;
            MusicAudioSource = _musicAudioSource;
        }
    }
}