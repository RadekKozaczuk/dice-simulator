#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        static readonly BallConfig _ballConfig;
        static readonly UIConfig _uiConfig;
        static readonly DiceConfig _diceConfig;

        [Inject]
        static readonly PresentationMainController _presentationMainController;

        [Preserve]
        PresentationViewModel() { }

        public static void CustomUpdate() => _presentationMainController.CustomUpdate();

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

            // load level data
            GetLevelSceneReferenceHolders();

            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Enable();

            PanelView ballsLeft = UISceneReferenceHolder.Panel;
            ballsLeft.gameObject.SetActive(true);
            ballsLeft.SetValues(0, 0);
        }

        public static void GameplayOnExit()
        {
            _uiConfig.InputActionAsset.FindActionMap(Constants.GameplayActionMap).Disable();
            UISceneReferenceHolder.Panel.gameObject.SetActive(false);

            PresentationData.Balls.Clear();
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

        static void GetLevelSceneReferenceHolders()
        {
            PresentationData.SceneReferenceHolders.Clear();
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LevelSceneReferenceHolder");

            foreach (GameObject go in gameObjects)
                PresentationData.SceneReferenceHolders.Add((Level)go.scene.buildIndex, go.GetComponent<LevelSceneReferenceHolder>());
        }
    }
}