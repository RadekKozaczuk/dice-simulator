#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using Core;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace GameLogic.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct UpdateDiceSystem : ISystem
    {
        void OnCreate(ref SystemState state) => state.RequireForUpdate<DiceComponent>();

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRO<LocalTransform> transform, RefRW<PhysicsVelocity> velocity, Entity entity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PhysicsVelocity>>()
                                 .WithAll<DiceComponent>()
                                 .WithChangeFilter<LocalTransform>()
                                 .WithEntityAccess())
            {
                bool isStopped = Utils.IsStopped(in velocity.ValueRO, 0.2f);

                if (isStopped)
                {
                    Signals.DiceStopped();

                    // freeze further movement
                    ecb.RemoveComponent<PhysicsVelocity>(entity);
                }
                else
                    Signals.DicePositionChanged(transform.ValueRO.Position, transform.ValueRO.Rotation);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}