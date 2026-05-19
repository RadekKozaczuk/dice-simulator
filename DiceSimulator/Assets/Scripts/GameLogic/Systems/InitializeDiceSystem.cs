using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core;
using Core.Dtos;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace GameLogic.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct InitializeDiceSystem : ISystem
    {
        void OnCreate(ref SystemState state) => state.RequireForUpdate<NewlySpawnedTag>();

        readonly void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRO<LocalTransform> transform, RefRO<DiceComponent> dice, Entity entity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRO<DiceComponent>>()
                                 .WithAll<NewlySpawnedTag>()
                                 .WithEntityAccess())
            {
#if UNITY_EDITOR
                ecb.SetName(entity, "Dice");
#endif

                // initially the dice is frozen
                ecb.RemoveComponent<PhysicsVelocity>(entity);
                ecb.RemoveComponent<NewlySpawnedTag>(entity);

                float3 position = transform.ValueRO.Position;
                quaternion rotation = transform.ValueRO.Rotation;
                var faces = new List<DiceFace>();

                foreach (DiceFace face in dice.ValueRO.Faces)
                    faces.Add(face);

                Signals.DiceSpawned(position, rotation, faces);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}