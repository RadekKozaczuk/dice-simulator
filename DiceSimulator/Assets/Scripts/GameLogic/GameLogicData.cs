#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Unity.Entities;
using Unity.Mathematics;

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
                // ReSharper disable once UnusedVariable
                var mouseClickPosition = new float2(
                    value.x * Constants.MapSizeX / 1920 - Constants.MapSizeX / 2,
                    value.y * Constants.MapSizeY / 1080 - Constants.MapSizeY / 2);

                // ReSharper disable once UnusedVariable
                World world = World.DefaultGameObjectInjectionWorld;

                /*SystemHandle handle = world.GetExistingSystem<SpawnBallSystem>();
                ref SystemState state = ref world.Unmanaged.ResolveSystemStateRef(handle);
                ref SpawnBallSystem system = ref world.Unmanaged.GetUnsafeSystemRef<SpawnBallSystem>(handle);
                system.SpawnBalls(ref state, mouseClickPosition);*/
            }
        }
    }
}