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

        static readonly Plane _movePlane = new(Vector3.up, new Vector3(0, 10, 0));

        // The maximum possible magnitude of a Vector2(1000, 1000)
        static readonly float _maxPossibleInputMag = Mathf.Sqrt(1000f * 1000f + 1000f * 1000f); // ~1414.21f
        const float MaxTargetOutput = 50f;

        [Inject]
        static readonly PresentationMainController _mainController;

        [Preserve]
        PresentationViewModel() { }

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        public static void MainMenuOnEntry()
        {
            InputService.Initialize();
            MusicService.LoadAndPlayWhenReady(Music.MainMenu, false);
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
            _uiConfig.InputActionAsset.FindActionMap(Constants.MainMenuActionMap).Enable();
            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Disable();
        }

        public static void GameplayOnEntry()
        {
            _uiConfig.InputActionAsset.FindActionMap(Constants.MainMenuActionMap).Disable();
            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Enable();

            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);

            PanelView panel = UISceneReferenceHolder.Panel;
            panel.gameObject.SetActive(true);
            panel.RollEnded(0, 0);
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

        public static void SetMousePosition(Vector2 mousePosition) => UpdateDrag(mousePosition);

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

        // todo: should be taken from logic
        static readonly float _minX = -10f;
        static readonly float _maxX = 10f;
        static readonly float _minZ = -10f;
        static readonly float _maxZ = 10f;

        static void UpdateDrag(Vector2 mousePosition)
        {
            Vector2 delta = mousePosition - _lastMousePosition;

            if (_isDragging)
            {
                Ray ray = PresentationSceneReferenceHolder.GameplayCamera.ScreenPointToRay(mousePosition);

                if (_movePlane.Raycast(ray, out float distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    hitPoint.x = Mathf.Clamp(hitPoint.x, _minX, _maxX);
                    hitPoint.z = Mathf.Clamp(hitPoint.z, _minZ, _maxZ);
                    GameLogicViewModel.MoveDice(hitPoint.x, hitPoint.z);
                }
            }

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

            float currentMagnitude = averageVelocity.magnitude;
            float percentage = Mathf.InverseLerp(0f, _maxPossibleInputMag, currentMagnitude);
            float mappedValue = Mathf.Lerp(0f, MaxTargetOutput, percentage);

            GameLogicViewModel.StartRoll(normal, mappedValue);
            PanelView panel = UISceneReferenceHolder.Panel;
            panel.RollInProgress();
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