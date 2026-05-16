#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using Core;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GameLogic.Systems
{
    /// <summary>
    /// Looks for all entities with <see cref="NewlySpawnedTag"/>
    /// and sends a signal and/or rpc to inform other systems that a bew prop has been spawned.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct InitializeBrickSystem : ISystem
    {
        void OnCreate(ref SystemState state) => state.RequireForUpdate<NewlySpawnedTag>();

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRW<LocalTransform> transform, RefRO<BrickComponent> brick, Entity entity)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BrickComponent>>()
                                 .WithAll<NewlySpawnedTag>()
                                 .WithEntityAccess())
            {
#if UNITY_EDITOR
                ecb.SetName(entity, $"{brick.ValueRO.Type}_{brick.ValueRO.Id}");
#endif

                float3 pos = transform.ValueRO.Position;
                float rotation = brick.ValueRO.Rotation;
                float scale = brick.ValueRO.Scale;
                ecb.RemoveComponent<NewlySpawnedTag>(entity);

                if (!state.EntityManager.HasComponent<InvisibilityTag>(entity))
                    Signals.BrickSpawned(brick.ValueRO.Id, brick.ValueRO.Type, pos.xz, rotation, scale, brick.ValueRO.Hp);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}