#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using Core;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GameLogic.Systems
{
    /// <summary>
    /// Iterates over all balls and destroys those tagged with <see cref="DestroyedTag"/>.
    /// Destruction in ECS is for some reason always postponed by one frame.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct SpawnDiceSystem : ISystem
    {
        void OnCreate(ref SystemState state) => state.RequireForUpdate<NewlySpawnedTag>();

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRO<LocalTransform> transform, Entity entity)
                     in SystemAPI.Query<RefRO<LocalTransform>>()
                                 .WithAll<DiceComponent, NewlySpawnedTag>()
                                 .WithEntityAccess())
            {
#if UNITY_EDITOR
                ecb.SetName(entity, "Dice");
#endif

                Debug.LogError("Dice spawned ECS");
                ecb.RemoveComponent<NewlySpawnedTag>(entity);
                Signals.DiceSpawned(transform.ValueRO.Position, transform.ValueRO.Rotation);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}