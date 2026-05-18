using System.Diagnostics.CodeAnalysis;
using Core;
using GameLogic.Config;
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
    partial struct UpdateDiceSystem : ISystem
    {
        static readonly PlayerConfig _config;

        void OnCreate(ref SystemState state) => state.RequireForUpdate<DiceTag>();

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRO<LocalTransform> transform, RefRW<PhysicsVelocity> velocity, Entity entity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PhysicsVelocity>>()
                                 .WithAll<DiceTag>()
                                 .WithChangeFilter<LocalTransform>()
                                 .WithEntityAccess())
            {
                bool isStopped = Utils.IsStopped(in velocity.ValueRO, _config.DicePhysicsFreezeThreshold);

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

        /// <summary>
        /// Moves the dice up and add a velocity component to it.
        /// </summary>
        internal void StartRoll(ref SystemState state, float2 direction)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity dice = SystemAPI.GetSingletonEntity<DiceTag>();
            RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(dice);
            transform.ValueRW.Position.y = _config.DiceHeight;

            float3 linear = new float3(direction.x, 0, direction.y) * _config.AutoRollStrength;
            var velocity = new PhysicsVelocity { Linear = linear };
            ecb.AddComponent(dice, velocity);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}