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
    /// Handles all collision triggers that happens in the game.
    /// Uses <see cref="DestroyedTag"/> to avoid duplicated triggers (when entity A and B collides both rise a trigger).
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [SuppressMessage("ReSharper", "MemberHidesInterfaceMemberWithDefaultImplementation")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    partial struct UpdateTriggerSystem : ISystem
    {
        // if we want to access something that is not part of the query we can use a component-lookup.
        // component-lookup is essentially a dictionary, so it introduces a random memory access operation therefore is it less efficient
        ComponentLookup<DiceComponent> _ballLookup;
        ComponentLookup<DestructionAreaTag> _destructionAreaLookup;

        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<DiceComponent>();

            _ballLookup = SystemAPI.GetComponentLookup<DiceComponent>(true);
            _destructionAreaLookup = SystemAPI.GetComponentLookup<DestructionAreaTag>(true);
        }

        void OnUpdate(ref SystemState state)
        {
            _ballLookup.Update(ref state);
            _destructionAreaLookup.Update(ref state);

            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

            // for destruction, we want to use a dedicated command buffer
            // this system runs at the end of the SimulationSystemGroup
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            foreach (TriggerEvent trigger in simulation.AsSimulation().TriggerEvents)
            {
                (Entity entityA, Entity _, CollisionEntityType typeA, CollisionEntityType typeB) =
                    IdentifyEntities(trigger, out bool important);

                // unimportant event
                if (!important)
                    continue;

                // ball triggered destruction area
                if (typeA == CollisionEntityType.Ball && typeB == CollisionEntityType.DestructionArea)
                {
                    if (entityManager.HasComponent<DestroyedTag>(entityA))
                        continue;

                    ecb.AddComponent(entityA, new DestroyedTag());
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }

        (Entity entityA, Entity entityB, CollisionEntityType typeA, CollisionEntityType typeB) IdentifyEntities(
            TriggerEvent trigger, out bool important)
        {
            // EntityA is a ball
            if (_ballLookup.HasComponent(trigger.EntityA))
                // ball triggered with a destruction area
                if (_destructionAreaLookup.HasComponent(trigger.EntityB))
                {
                    important = true;
                    return (trigger.EntityA, trigger.EntityB, CollisionEntityType.Ball, CollisionEntityType.DestructionArea);
                }

            // EntityA is a destruction area
            if (_destructionAreaLookup.HasComponent(trigger.EntityA))
                // destruction area triggered with a ball
                if (_ballLookup.HasComponent(trigger.EntityB))
                {
                    // destruction area collided with a ball
                    important = true;
                    return (trigger.EntityB, trigger.EntityA, CollisionEntityType.Ball, CollisionEntityType.DestructionArea);
                }

            // insignificant event like for example player collided with another player
            important = false;
            return (Entity.Null, Entity.Null, CollisionEntityType.Undefined, CollisionEntityType.Undefined);
        }
    }
}