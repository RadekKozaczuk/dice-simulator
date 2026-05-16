#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using GameLogic.Dtos;
using GameLogic.Systems;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace GameLogic
{
    /// <summary>
    /// Assembly-level data. Everything here should be internal.
    /// </summary>
    static class GameLogicData
    {
        /// <summary>
        /// Indicates that the player clicked on the given pixel this frame.
        /// The value is the exact pixel coordinate where (0, 0) is the lower-left corner
        /// and, (1920, 1080) (depending on the resolution), is the upper right corner.
        /// Reset to null on the end of every frame.
        /// </summary>
        internal static float2 MouseClickPosition
        {
            set
            {
                // do nothing if there are still previous balls flight around or no more shots left
                if (BallsLeft <= 0 || BallDtos.Count > 0)
                    return;
                
                var mouseClickPosition = new float2(
                    value.x * Constants.MapSizeX / 1920 - Constants.MapSizeX / 2,
                    value.y * Constants.MapSizeY / 1080 - Constants.MapSizeY / 2);
                
                var world = World.DefaultGameObjectInjectionWorld;

                SystemHandle handle = world.GetExistingSystem<SpawnBallSystem>();
                ref SystemState state = ref world.Unmanaged.ResolveSystemStateRef(handle);
                ref SpawnBallSystem system = ref world.Unmanaged.GetUnsafeSystemRef<SpawnBallSystem>(handle);
                system.SpawnBalls(ref state, mouseClickPosition);

                BallsLeft--;
            }
        }

        /// <summary>
        /// Amount of shots the player still has at his disposal.
        /// </summary>
        internal static int BallsLeft
        {
            get => _ballsLeft;
            set
            {
                Assert.IsTrue(value >= 0, "BallsLeft must be greater than or equal to 0");
                _ballsLeft = value;
                Signals.BallsLeftChanged(_ballsLeft);
            }
        }
        static int _ballsLeft;

        internal static readonly Dictionary<int, BallDto> BallDtos = new();
    }
}