#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Entities;

namespace GameLogic.Components
{
    struct BallComponent : IComponentData
    {
        internal readonly int Id;

        internal BallComponent(int id) => Id = id;
    }
}