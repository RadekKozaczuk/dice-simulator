#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using GameLogic.Components;
using GameLogic.Config;
using GameLogic.Dtos;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace GameLogic.Systems
{
    /// <summary>
    /// This system copies values from <see cref="GameLogicData"/> to the player's input component.
    /// The variables are then reset.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct SpawnBallSystem : ISystem
    {
        static readonly PlayerConfig _config;

        static int _nextBallId; // todo: should use id counter, or just entity ID as this is not multiplayer
        // Note that while zero-size component may be created, they can not be accessed directly in code.
        // They are only usable for situations such as in calling RequireForUpdate<T>().

        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnPointTag>();
            state.RequireForUpdate<PrefabsComponent>();
        }

        void OnUpdate(ref SystemState state) => state.Enabled = false;

        internal void SpawnBalls(ref SystemState state, float2 mousePosition)
        {
            Entity singleton = SystemAPI.GetSingletonEntity<SpawnPointTag>();
            RefRO<LocalTransform> spawn = SystemAPI.GetComponentRO<LocalTransform>(singleton); 

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var prefabs = SystemAPI.GetSingleton<PrefabsComponent>();
            float3 position = spawn.ValueRO.Position;

            // calculate shot direction
            float2 direction = math.normalizesafe(mousePosition - position.xz);
            float deltaTime = SystemAPI.Time.DeltaTime;

            for (int i = 0; i < _config.BallPerShotCount; i++)
            {
                Entity ball = ecb.Instantiate(prefabs.Ball);
                ecb.SetComponent(ball, new LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                int id = _nextBallId++;

#if UNITY_EDITOR
                ecb.SetName(ball, $"Ball_{id}");
#endif

                ecb.SetComponent(ball, new BallComponent(id));
                float3 velocity = new float3(direction.x, 0, direction.y) * 100 * _config.BallSpeed * deltaTime;
                ecb.SetComponent(ball, new PhysicsVelocity { Linear = velocity });

                GameLogicData.BallDtos.Add(id, new BallDto(id, position.xz));
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}