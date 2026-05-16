#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using GameLogic.Config;
using GameLogic.Services;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public class GameLogicViewModel
    {
        static readonly PlayerConfig _config;
        
        /// <summary>
        /// This value is a pixel coordinate where (0, 0) is the lower-left corner
        /// and, (1920, 1080) (depending on the resolution), is the upper right corner.
        /// </summary>
        public static Vector2 MouseClickPosition { set => GameLogicData.MouseClickPosition = value; }

        [Preserve]
        GameLogicViewModel() { }

        public static void CustomUpdate() { }

        public static void BootingOnExit() => PersistentStorageService.Initialize();

        public static void MainMenuOnEntry() { }

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry()
        {
            CoreData.Score = 0;
            GameLogicData.BallsLeft = _config.BallPerGameCount;
        }

        public static void GameplayOnExit() { }

        public static void SaveVolumeSettings(int music, int sound) => PersistentStorageService.SaveVolumeSettings(music, sound);

        public static (int music, int sound) LoadVolumeSettings() => PersistentStorageService.LoadVolumeSettings();

        /// <summary>
        /// If the instance hosted a lobby, the lobby will be deleted.
        /// </summary>
        public static void QuitGame() { }
    }
}