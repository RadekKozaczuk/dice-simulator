using System.Diagnostics.CodeAnalysis;
using Core;
using Core.Dtos;
using GameLogic.Components;
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
        int _total;

        void OnCreate(ref SystemState state) => state.RequireForUpdate<DiceComponent>();

        void OnUpdate(ref SystemState state)
        {
            // update position
            foreach (RefRO<LocalTransform> transform in SystemAPI.Query<RefRO<LocalTransform>>()
                                                                 .WithAll<DiceComponent>()
                                                                 .WithChangeFilter<LocalTransform>())
                Signals.DicePositionChanged(transform.ValueRO.Position, transform.ValueRO.Rotation);

            // freeze if necessary
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach ((RefRO<LocalTransform> transform, RefRW<PhysicsVelocity> velocity, RefRO<DiceComponent> dice, Entity entity)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<DiceComponent>>()
                                 .WithChangeFilter<LocalTransform>()
                                 .WithEntityAccess())
            {
                bool isStopped = Utils.IsStopped(in velocity.ValueRO, _config.DicePhysicsFreezeThreshold);

                if (isStopped)
                {
                    int result = GetBestResult(in dice.ValueRO.Faces, in transform.ValueRO.Rotation);
                    _total += result;
                    Signals.DiceStopped(result, _total);

                    ecb.RemoveComponent<PhysicsVelocity>(entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        internal void MoveDice(ref SystemState state, float deltaX, float deltaZ)
        {
            Entity dice = SystemAPI.GetSingletonEntity<DiceComponent>();
            RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(dice);
            transform.ValueRW.Position = new float3(deltaX, _config.DiceHeight, deltaZ);
        }

        /// <summary>
        /// Moves the dice up and add a velocity component to it.
        /// </summary>
        internal void StartRoll(ref SystemState state, float2 direction, float magnitude, bool resetPosition)
        {
            float3 linear = new float3(direction.x, 0, direction.y) * magnitude;
            StartRoll(ref state, linear, resetPosition);
        }

        void StartRoll(ref SystemState state, float3 linear, bool resetPosition)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            Entity dice = SystemAPI.GetSingletonEntity<DiceComponent>();

            if (resetPosition)
            {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(dice);
                transform.ValueRW.Position = new float3(0, _config.DiceHeight, 0);
            }

            var velocity = new PhysicsVelocity { Linear = linear };
            ecb.AddComponent(dice, velocity);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        int GetBestResult(in FixedList512Bytes<DiceFace> faces, in quaternion rotation)
        {
            float bestDot = -1f;
            int bestIndex = -1;

            for (int i = 0; i < faces.Length; i++)
            {
                DiceFace face = faces[i];

                // Convert local normal to world space
                float3 worldNormal = math.rotate(rotation, face.Normal);

                // Compare with world up
                float dot = math.dot(worldNormal, math.up());

                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestIndex = i;
                }
            }

            return faces[bestIndex].Number;
        }
    }
}