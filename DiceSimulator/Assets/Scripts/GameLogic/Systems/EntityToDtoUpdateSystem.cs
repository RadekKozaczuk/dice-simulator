#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace GameLogic.Systems
{
    /// <summary>
    /// After all calculation are completed it is time to copy over all the data to dtos.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct EntityToDtoUpdateSystem : ISystem
    {
        public void OnCreate(ref SystemState state) => state.RequireForUpdate<LocalTransform>();

        void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach ((RefRO<BallComponent> ball, RefRO<LocalTransform> transform)
                     in SystemAPI.Query<RefRO<BallComponent>, RefRO<LocalTransform>>()
                                 .WithAll<PhysicsVelocity>()
                                 .WithNone<DestroyedTag>())
                GameLogicData.BallDtos[ball.ValueRO.Id].Position = transform.ValueRO.Position.xz;

            ecb.Playback(state.EntityManager);
        }
    }
}