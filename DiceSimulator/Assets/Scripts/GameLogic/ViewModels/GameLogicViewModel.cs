#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using GameLogic.Config;
using GameLogic.Services;
using GameLogic.Systems;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;
using Random = Unity.Mathematics.Random;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public class GameLogicViewModel
    {
        static readonly PlayerConfig _config;

        static Random _random = new(123);

        /// <summary>
        /// This value is a pixel coordinate where (0, 0) is the lower-left corner
        /// and, (1920, 1080) (depending on the resolution), is the upper right corner.
        /// </summary>
        public static Vector2 MouseClickPosition { set => GameLogicData.MouseClickPosition = value; }

        [Preserve]
        GameLogicViewModel() { }

        public static void BootingOnExit() => PersistentStorageService.Initialize();

        public static void MainMenuOnEntry() { }

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry() { }

        public static void GameplayOnExit() { }

        public static void SaveVolumeSettings(int music, int sound) => PersistentStorageService.SaveVolumeSettings(music, sound);

        public static (int music, int sound) LoadVolumeSettings() => PersistentStorageService.LoadVolumeSettings();

        /// <summary>
        /// Dice will start to ascend and is ready to be casted.
        /// </summary>
        public static void DiceSelected() { }

        /// <summary>
        /// Dice will start to move back to start position.
        /// </summary>
        public static void DiceUnselected() { }

        public static void AutoRoll()
        {
            // randomize direction and strength
            float2 direction = _random.NextFloat2Direction();
            float magnitude = _random.NextFloat();

            StartRoll(direction, magnitude);
        }

        static void StartRoll(float2 direction, float magnitude)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle handle = world.GetExistingSystem<UpdateDiceSystem>();
            ref SystemState state = ref world.Unmanaged.ResolveSystemStateRef(handle);
            ref UpdateDiceSystem system = ref world.Unmanaged.GetUnsafeSystemRef<UpdateDiceSystem>(handle);
            system.StartRoll(ref state, direction);
        }

        public static void QuitGame() { }
    }
}