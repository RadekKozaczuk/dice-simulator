#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using Core;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;

namespace GameLogic.Systems
{
    /// <summary>
    /// Iterates over all balls and destroys those tagged with <see cref="DestroyedTag"/>.
    /// Destruction in ECS is for some reason always postponed by one frame.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateCollisionSystem))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct CleanupSystem : ISystem
    {
        void OnCreate(ref SystemState state) => state.RequireForUpdate<DestroyedTag>();

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRO<DestroyedTag> _, Entity entity)
                     in SystemAPI.Query<RefRO<DestroyedTag>>().WithEntityAccess())
            {
                if (state.EntityManager.HasComponent<BallComponent>(entity))
                {
                    int id = state.EntityManager.GetComponentData<BallComponent>(entity).Id;
                    Signals.BallDestroyed(id);

                    GameLogicData.BallDtos.Remove(id);
                    if (GameLogicData.BallDtos.Count == 0 && GameLogicData.BallsLeft == 0)
                        Signals.GameEnded();
                }
                else if (state.EntityManager.HasComponent<BrickComponent>(entity))
                {
                    int id = state.EntityManager.GetComponentData<BrickComponent>(entity).Id;
                    Signals.BrickDestroyed(id);
                }

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}