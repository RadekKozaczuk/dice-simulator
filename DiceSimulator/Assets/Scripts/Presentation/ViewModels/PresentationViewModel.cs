using Core.DependencyInjector;
using Core;
using GameLogic.ViewModels;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using Presentation.Services;
using Presentation.Views;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public class PresentationViewModel
    {
        static readonly UIConfig _uiConfig;
        static readonly PresentationConfig _presentationConfig;

        static bool _isDragging;
        static Vector2 _lastMousePosition;
        static readonly Vector2[] _velocityBuffer = new Vector2[30];
        static int _velocityIndex;

        // The maximum possible magnitude of a Vector2(1000, 1000)
        static readonly float _maxPossibleInputMag = Mathf.Sqrt(1000f * 1000f + 1000f * 1000f); // ~1414.21f
        const float MaxTargetOutput = 50f;

        [Inject]
        static readonly PresentationMainController _presentationMainController;

        [Preserve]
        PresentationViewModel() { }

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        public static void BootingOnExit() => InputService.Initialize();

        public static void MainMenuOnEntry()
        {
            MusicService.LoadAndPlayWhenReady(Music.MainMenu, false);
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
            _uiConfig.InputActionAsset.FindActionMap(Constants.MainMenuActionMap).Enable();
        }

        public static void MainMenuOnExit() => _uiConfig.InputActionAsset.FindActionMap(Constants.MainMenuActionMap).Disable();

        public static void GameplayOnEntry()
        {
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);

            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Enable();

            PanelView ballsLeft = UISceneReferenceHolder.Panel;
            ballsLeft.gameObject.SetActive(true);
            ballsLeft.SetValues(0, 0);
        }

        public static void GameplayOnExit()
        {
            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Disable();
            UISceneReferenceHolder.Panel.gameObject.SetActive(false);
        }

        public static void SetMusicVolume(int music)
        {
            Assert.IsTrue(music is >= 0 and <= 10, "Volume must be represented by a value randing from 0 to 10.");
            MusicService.Volume = music;
        }

        public static void SetSoundVolume(int sound)
        {
            Assert.IsTrue(sound is >= 0 and <= 10, "Volume must be represented by a value randing from 0 to 10.");
            SoundService.Volume = sound;
        }

        public static void PlaySound(Sound sound) => SoundService.Play(sound);

        public static void SetMousePosition(Vector2 mousePosition)
        {
            UpdateDrag(mousePosition);
        }

        public static void TryGrabDice(Vector2 mousePosition)
        {
            var position = new Vector3(mousePosition.x, mousePosition.y, 0f);
            Ray ray = PresentationSceneReferenceHolder.GameplayCamera.ScreenPointToRay(position);
            int mask = LayerMask.GetMask("Dice");

            if (Physics.Raycast(ray, out RaycastHit _, 1000f, mask))
                BeginDrag(mousePosition);
        }

        public static void ReleaseDice()
        {
            if (_isDragging)
                EndDrag();
        }

        static void BeginDrag(Vector2 mousePosition)
        {
            _isDragging = true;
            _lastMousePosition = mousePosition;

            for (int i = 0; i < _velocityBuffer.Length; i++)
                _velocityBuffer[i] = Vector2.zero;

            _velocityIndex = 0;
        }

        static void UpdateDrag(Vector2 mousePosition)
        {
            Vector2 delta = mousePosition - _lastMousePosition;

            // pixels per second
            Vector2 velocity = delta / Time.deltaTime;

            _velocityBuffer[_velocityIndex] = velocity;
            _velocityIndex = (_velocityIndex + 1) % _velocityBuffer.Length;

            _lastMousePosition = mousePosition;
        }

        static void EndDrag()
        {
            _isDragging = false;

            Vector2 averageVelocity = GetAverageVelocity();
            var normal = Vector2.Normalize(averageVelocity);

            Debug.LogError($"averageVelocity: {averageVelocity}");

            // 1. Get the actual length of the Vector2
            float currentMagnitude = averageVelocity.magnitude;

            // 2. Turn it into a 0.0 to 1.0 percentage based on the max diagonal limit
            float percentage = Mathf.InverseLerp(0f, _maxPossibleInputMag, currentMagnitude);

            // 3. Scale that percentage to your target range of 0 to 50
            float mappedValue = Mathf.Lerp(0f, MaxTargetOutput, percentage);

            Debug.LogError($"avg: {mappedValue}");

            GameLogicViewModel.StartRoll(normal, mappedValue);
        }

        static Vector2 GetAverageVelocity()
        {
            Vector2 sum = Vector2.zero;

            foreach (Vector2 v in _velocityBuffer)
                sum += v;

            return sum / _velocityBuffer.Length;
        }
    }
}