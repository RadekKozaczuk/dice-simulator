#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
using Core;
using GameLogic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace GameLogic.Systems
{
    /// <summary>
    /// Handles all collision events that happen in the game.
    /// Uses <see cref="DestroyedTag"/> to avoid duplicated events (when entity A and B collides both rise a trigger).
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateTriggerSystem))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct UpdateCollisionSystem : ISystem
    {
        // if we want to access something that is not part of the query we can use a component-lookup.
        // component-lookup is essentially a dictionary, so it introduces a random memory access operation therefore is it less efficient
        ComponentLookup<BallComponent> _ballLookup;
        ComponentLookup<BrickComponent> _brickLookup;

        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<BallComponent>();

            _ballLookup = SystemAPI.GetComponentLookup<BallComponent>(true);
            _brickLookup = SystemAPI.GetComponentLookup<BrickComponent>();
        }

        void OnUpdate(ref SystemState state)
        {
            _ballLookup.Update(ref state);
            _brickLookup.Update(ref state);

            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

            // for destruction, we want to use a dedicated command buffer
            // this system runs at the end of the SimulationSystemGroup
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            foreach (CollisionEvent collision in simulation.AsSimulation().CollisionEvents)
            {
                (Entity _, Entity entityB, CollisionEntityType typeA, CollisionEntityType typeB) =
                    IdentifyEntities(collision, out bool important);

                // unimportant event
                if (!important)
                    continue;

                // ball collided with a brick
                if (typeA == CollisionEntityType.Ball && typeB == CollisionEntityType.Brick)
                {
                    RefRW<BrickComponent> brick = SystemAPI.GetComponentRW<BrickComponent>(entityB);

                    if (entityManager.HasComponent<IndestructibleTag>(entityB))
                    {
                        Signals.BrickHit(brick.ValueRO.Id, brick.ValueRW.Hp);
                        continue;
                    }

                    brick.ValueRW.Hp--;
                    Signals.BrickHit(brick.ValueRO.Id, brick.ValueRW.Hp);
                    CoreData.Score++;

                    if (brick.ValueRO.Hp == 0)
                        ecb.AddComponent(entityB, new DestroyedTag());
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }

        (Entity entityA, Entity entityB, CollisionEntityType typeA, CollisionEntityType typeB) IdentifyEntities(
            CollisionEvent collision, out bool important)
        {
            // EntityA is a ball
            if (_ballLookup.HasComponent(collision.EntityA))
                // ball can only collide with a brick
                if (_brickLookup.HasComponent(collision.EntityB))
                {
                    important = true;
                    return (collision.EntityA, collision.EntityB, CollisionEntityType.Ball, CollisionEntityType.Brick);
                }

            // EntityA is a brick - check if EntityB is a ball
            if (_brickLookup.HasComponent(collision.EntityA))
                // brick can only collide with balls
                if (_ballLookup.HasComponent(collision.EntityB))
                {
                    important = true;
                    return (collision.EntityB, collision.EntityA, CollisionEntityType.Ball, CollisionEntityType.Brick);
                }

            // insignificant event
            important = false;
            return (Entity.Null, Entity.Null, CollisionEntityType.Undefined, CollisionEntityType.Undefined);
        }
    }
}