#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Entities;

// Adding/Removing tags do not cause structural changes making them very efficient
namespace GameLogic
{
    struct NewlySpawnedTag : IComponentData { }
}