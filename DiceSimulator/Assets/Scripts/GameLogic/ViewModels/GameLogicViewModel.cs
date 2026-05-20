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

        [Preserve]
        GameLogicViewModel() { }

        public static void MainMenuOnEntry()
        {
            PersistentStorageService.Initialize();
        }

        public static void GameplayOnEntry() { }

        public static void SaveVolumeSettings(int music, int sound) =>
            PersistentStorageService.SaveVolumeSettings(music, sound);

        public static (int music, int sound) LoadVolumeSettings() =>
            PersistentStorageService.LoadVolumeSettings();

        public static void MoveDice(float x, float z)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle handle = world.GetExistingSystem<UpdateDiceSystem>();
            ref SystemState state = ref world.Unmanaged.ResolveSystemStateRef(handle);
            ref UpdateDiceSystem system = ref world.Unmanaged.GetUnsafeSystemRef<UpdateDiceSystem>(handle);
            system.MoveDice(ref state, x, z);
        }

        /// <summary>
        /// Rolls the dice in a random direction with a predetermined (<see cref="PlayerConfig.AutoRollStrength"/>) strength.
        /// </summary>
        public static void AutoRoll()
        {
            // randomize direction and strength
            float2 direction = _random.NextFloat2Direction();
            StartRoll_Internal(direction, _config.AutoRollStrength);
        }

        public static void StartRoll(Vector2 direction, float magnitude) =>
            StartRoll_Internal(direction, magnitude);

        static void StartRoll_Internal(float2 direction, float magnitude)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle handle = world.GetExistingSystem<UpdateDiceSystem>();
            ref SystemState state = ref world.Unmanaged.ResolveSystemStateRef(handle);
            ref UpdateDiceSystem system = ref world.Unmanaged.GetUnsafeSystemRef<UpdateDiceSystem>(handle);
            system.StartRoll(ref state, direction, magnitude);
        }

        public static void QuitGame() { }
    }
}