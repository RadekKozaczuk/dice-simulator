#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Entities;

// Adding/Removing tags do not cause structural changes making them very efficient
namespace GameLogic
{
    /// <summary>
    /// Means that this entity is scheduled to be destroyed.
    /// Sometimes for various reasons it is better to tag entities as "to-be-destroyed" and destroy them in a different system.
    /// Instead of destroying them right away.
    /// </summary>
    struct DestroyedTag : IComponentData { }
    
    struct NewlySpawnedTag : IComponentData { }
    
    struct DestructionAreaTag : IComponentData { }
    
    struct IndestructibleTag : IComponentData { }

    struct InvisibilityTag : IComponentData { }
    
    struct SpawnPointTag : IComponentData { }
}