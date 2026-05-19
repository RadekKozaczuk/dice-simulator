using System.Collections.Generic;
using Core.DependencyInjector;
using Core;
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
        static List<Vector2> _velocityBuffer = new();
        static int _velocityIndex;

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

        public static void SetMousePosition(Vector2 mousePosition) { }

        public static void TryGrabDice(Vector2 mousePosition)
        {
            var position = new Vector3(mousePosition.x, mousePosition.y, 0f);
            Ray ray = PresentationSceneReferenceHolder.GameplayCamera.ScreenPointToRay(position);
            int mask = LayerMask.GetMask("Dice");

            Vector3 missEndPoint = ray.origin + ray.direction * 1000f;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mask))
            {
                //Debug.DrawLine(ray.origin, missEndPoint, Color.green, 999f);
                Debug.Log("Clicked: " + hit.collider.name);
                BeginDrag(mousePosition);
            }
            /*else
            {
                // Fix: Draw the line along the ray's direction for its max distance (1000f)
                Debug.DrawLine(ray.origin, missEndPoint, Color.red, 999f);
            }*/
        }

        public static void ReleaseDice() { }

        static void BeginDrag(Vector2 mousePosition)
        {
            _isDragging = true;

            _lastMousePosition = mousePosition;

            for (int i = 0; i < _velocityBuffer.Count; i++)
                _velocityBuffer[i] = Vector2.zero;

            _velocityIndex = 0;
        }

        void UpdateDrag(Vector2 mousePosition)
        {
            Vector2 currentMousePosition = mousePosition;

            Vector2 delta = currentMousePosition - _lastMousePosition;

            // pixels per second
            Vector2 velocity = delta / Time.deltaTime;

            _velocityBuffer[_velocityIndex] = velocity;
            _velocityIndex = (_velocityIndex + 1) % _velocityBuffer.Count;

            _lastMousePosition = currentMousePosition;
        }

        void EndDrag()
        {
            _isDragging = false;

            Vector2 averageVelocity = GetAverageVelocity();

            //RollDice(averageVelocity);
        }

        static Vector2 GetAverageVelocity()
        {
            Vector2 sum = Vector2.zero;

            foreach (Vector2 v in _velocityBuffer)
                sum += v;

            return sum / _velocityBuffer.Count;
        }
    }
}