using System.Diagnostics.CodeAnalysis;
using Core;
using Unity.Collections;
using Unity.Entities;
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

            foreach ((RefRO<LocalTransform> transform, Entity entity)
                     in SystemAPI.Query<RefRO<LocalTransform>>()
                                 .WithAll<DiceTag, NewlySpawnedTag>()
                                 .WithEntityAccess())
            {
#if UNITY_EDITOR
                ecb.SetName(entity, "Dice");
#endif

                // initially the dice is frozen
                ecb.RemoveComponent<PhysicsVelocity>(entity);
                ecb.RemoveComponent<NewlySpawnedTag>(entity);

                Signals.DiceSpawned(transform.ValueRO.Position, transform.ValueRO.Rotation);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}