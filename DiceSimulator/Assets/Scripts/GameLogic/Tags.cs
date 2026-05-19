using Unity.Entities;

// Adding/Removing tags do not cause structural changes making them very efficient
namespace GameLogic
{
    struct NewlySpawnedTag : IComponentData { }
}