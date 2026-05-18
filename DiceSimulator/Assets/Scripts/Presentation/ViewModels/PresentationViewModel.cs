using Core.DependencyInjector;
using Core;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using Presentation.Services;
using Presentation.Views;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public class PresentationViewModel
    {
        static readonly UIConfig _uiConfig;
        static readonly PresentationConfig _presentationConfig;

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
    }
}